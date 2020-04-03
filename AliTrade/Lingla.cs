using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace AliTrade
{
    public class Lingla
    {
        private static string ecoUrl = "https://eco.taobao.com/router/rest";
        private static string taeUrl = "http://tae.liqu.com/api/Client/Tae";
        private const string app_key = "24526511";
        private const string app_secret = "b90a00845698ab39e500d2a705161927";
        private const long adzoneId = 109039050005L;
        private const string SIGN_METHOD_MD5 = "md5";
        private static string MD5(string s, Encoding e)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            return BitConverter.ToString(md5.ComputeHash(e.GetBytes(s))).Replace("-", string.Empty).ToLower();
        }

        private T Execute<T>(ITaeRequest<T> request) where T : TaeResponse
        {
            var txtParams = request.GetParameters();
            var taeReq = request.taeReq();
            var APPKEY = app_key; var APPSECRET = app_secret;
            txtParams.Add("method", request.GetApiName());
            if (taeReq)
            {
                txtParams.Add("appkey", APPKEY);
                txtParams.Add("secret", APPSECRET);
            }
            else
            {
                txtParams.Add("app_key", APPKEY);
                txtParams.Add("sign_method", SIGN_METHOD_MD5);
                txtParams.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                txtParams.Add("format", "json");
                txtParams.Add("v", "2.0");
                txtParams.Add("simplify", "true");
                #region 签名(md5)算法
                IDictionary<string, string> sortParams = new SortedDictionary<string, string>(txtParams, StringComparer.Ordinal);
                var sign = string.Empty;
                foreach (KeyValuePair<string, string> kv in sortParams)
                {
                    string name = kv.Key; string value = kv.Value;
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
                    {
                        sign += name + value;
                    }
                }
                sign = APPSECRET + sign + APPSECRET;
                sign = MD5(sign, Encoding.UTF8);
                #endregion
                txtParams.Add("sign", sign.ToUpper());
            }
            var url = (taeReq ? taeUrl : ecoUrl);
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            handler.AllowAutoRedirect = true;
            #region UseCookies
            //handler.UseCookies = true;
            //handler.CookieContainer = null;
            #endregion
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMilliseconds(1 * 60 * 1000);
            #region RequestHeaders
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            #endregion
            HttpResponseMessage response = null;
            T rsp = Activator.CreateInstance<T>();
            try
            {
                if (taeReq)
                {
                    url = url + (url.Contains("?") ? "&" : "?") + "taereq=" + request.GetApiName() + "&taeappkey=" + APPKEY;
                    string content = JsonConvert.SerializeObject(new { Body = txtParams });
                    var stringContent = new StringContent(content, Encoding.UTF8, "application/json");
                    response = client.PostAsync(url, stringContent).Result;
                }
                else
                {
                    response = client.PostAsync(url, new FormUrlEncodedContent(txtParams)).Result;
                }
                var iResult = response.Content.ReadAsStringAsync().Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = JsonConvert.DeserializeObject<IDictionary>(iResult);
                    if (json.Contains("error_response"))
                    {
                        var errRsp = new ErrorResponse();
                        foreach (object key in json.Keys)
                        {
                            errRsp = JsonConvert.DeserializeObject<ErrorResponse>(json[key].ToString());
                            break;
                        }
                        rsp.error_code = "1000001";
                        rsp.error_msg = errRsp.sub_msg;
                        rsp.TopErrResp = errRsp;
                        Console.WriteLine("tae.Execute【" + request.GetApiName() + "】 error_response >> " + iResult);
                    }
                    else
                    {
                        // txtParams.Add("simplify","true");
                        // 已经是精简版json，无需判断resp
                        rsp = JsonConvert.DeserializeObject<T>(iResult);
                    }
                }
                else
                {
                    Console.WriteLine("tae.Execute." + request.GetApiName() + " 非200 >> " + iResult);
                }
                rsp.Body = iResult;
            }
            catch (System.Exception ex)
            {
                rsp.error_code = "1000009";
                rsp.error_msg = ex.Message;
                Console.WriteLine("tae.Execute." + request.GetApiName() + " 异常 >> " + ex.Message);
            }
            return rsp;
        }

        /// <summary>
        /// 获取联盟订单
        /// </summary>
        /// <param name="action">事务</param>
        /// <param name="dt">查询时间（向前查询20分钟）；对应quertype的时间</param>
        /// <param name="query_type">1：下单状态；3：结算状态</param>
        /// <returns></returns>
        public void GetQueryTrade(Action<TbkOrderDetailsQueryResult> action, DateTime dt, int query_type = 1)
        {
            var start_time = dt.AddMinutes(-20);
            var end_time = dt;
            var tk_status = 0;
            var position_index = "";
            var page = 1;
            while (true)
            {
                var dataResult = GetTbkOrderDetails(start_time, end_time, tk_status, query_type, 0, 1, 1, position_index, page, 100);
                if (dataResult == null || dataResult.results == null || dataResult.results.Count == 0)
                    break;
                foreach (var item in dataResult.results)
                {
                    if (item.adzone_id == adzoneId)
                    {
                        action.Invoke(item);
                    }
                }
                position_index = dataResult.position_index;
                page++;
                if (!dataResult.has_next)
                {
                    break;
                }
            }
        }
        /// <summary>
        /// 获取联盟订单
        /// </summary>
        /// <param name="dt">查询时间（向前查询20分钟）；对应quertype的时间</param>
        /// <param name="query_type">1：下单状态；3：结算状态</param>
        /// <returns></returns>
        public List<TbkOrderDetailsQueryResult> GetQueryTrade(DateTime dt, int query_type = 1)
        {
            var result = new List<TbkOrderDetailsQueryResult>();
            var start_time = dt.AddMinutes(-20); var end_time = dt;
            var tk_status = 0;
            var position_index = "";
            var page = 1;
            while (true)
            {
                var dataResult = GetTbkOrderDetails(start_time, end_time, tk_status, query_type, 0, 1, 1, position_index, page, 100);
                if (dataResult == null || dataResult.results == null || dataResult.results.Count == 0)
                    break;
                foreach (var item in dataResult.results)
                {
                    if (item.adzone_id == adzoneId)
                    {
                        result.Add(item);
                    }
                }
                position_index = dataResult.position_index;
                page++;
                if (!dataResult.has_next)
                {
                    break;
                }
            }
            return result;
        }
        public TbkOrderDetailsQueryData GetTbkOrderDetails(DateTime start_time, DateTime end_time, int tk_status = 0,
                int query_type = 1, string position_index = "", int page_no = 1, int page_size = 100)
        {
            return GetTbkOrderDetails(start_time, end_time, tk_status, query_type, 0, 1, 1, position_index, page_no, page_size);
        }
        public TbkOrderDetailsQueryData GetTbkOrderDetails(DateTime start_time, DateTime end_time, int tk_status = 0,
                int query_type = 1, int member_type = 0, int order_scene = 1,
                int jump_type = 1, string position_index = "", int page_no = 1, int page_size = 100)
        {
            var req = new TbkOrderDetailsQueryRequest();
            req.app_key = app_key;
            req.app_secret = app_secret;

            req.start_time = start_time;
            req.end_time = end_time;
            req.tk_status = tk_status;

            req.query_type = query_type;
            req.member_type = member_type;
            req.order_scene = order_scene;

            req.jump_type = jump_type;
            req.position_index = position_index;
            req.page_no = page_no;
            req.page_size = page_size;
            var result = Execute(req);
            if (!result.IsError)
            {
                return result.data;
            }
            return null;
        }
    }
}
