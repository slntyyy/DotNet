using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApiClient.Controllers.Models.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QRCoder;
using System.Drawing;
using System.IO;

namespace ApiClient.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        public ClientController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Tae接口调用方法
        /// </summary>
        /// <remarks>
        /// remarks
        /// </remarks>
        /// <param></param>
        /// <returns></returns>
        /// <response code="400">400</response>
        /// <response code="404">404</response>
        [Route("Tae"), HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TaeClient([FromBody]TaeRequest request)
        {
            if (request == null || request.Body.Count <= 0)
            {
                return NotFound();
            }

            var appkey = "23363435";
            var secret = "145d10563c369cc0bda14f81124c0c1c";
            var url = "https://eco.taobao.com/router/rest";
            if (request.Body.ContainsKey("appkey"))
            {
                appkey = request.Body["appkey"].Trim();
                request.Body.Remove("appkey");
            }
            if (request.Body.ContainsKey("secret"))
            {
                secret = request.Body["secret"].Trim();
                request.Body.Remove("secret");
            }
            var txtParams = new Dictionary<string, string>();
            txtParams.Add("app_key", appkey);
            txtParams.Add("sign_method", "md5");
            txtParams.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            txtParams.Add("format", "json");
            txtParams.Add("v", "2.0");
            txtParams.Add("simplify", "true");
            foreach (var p in request.Body)
            {
                if (txtParams.ContainsKey(p.Key))
                {
                    txtParams[p.Key] = p.Value;
                }
                else
                {
                    txtParams.Add(p.Key, p.Value);
                }
            }
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
            sign = secret + sign + secret;
            sign = MD5(sign, Encoding.UTF8);
            #endregion
            txtParams.Add("sign", sign.ToUpper());
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            handler.AllowAutoRedirect = true;
            var client = new HttpClient(handler);
            client.Timeout = TimeSpan.FromMilliseconds(1 * 60 * 1000);
            //client.BaseAddress = Uri;
            #region RequestHeaders
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            #endregion
            HttpResponseMessage response = null;
            try
            {
                response = client.PostAsync(url, new FormUrlEncodedContent(txtParams)).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var iResult = await response.Content.ReadAsStringAsync();
                    var objResult = System.Text.Json.JsonSerializer.Deserialize<object>(json: iResult);
                    return Ok(objResult);
                }
            }
            catch (Exception)
            {
                // logger
                return BadRequest();
            }
            return NotFound();
        }

        /// <summary>
        /// Acs接口调用方法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("Acs"), HttpPost]
        public async Task<IActionResult> AcsClient(AcsRequest request)
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip;
            handler.AllowAutoRedirect = true;
            #region UseCookies
            if (request.Cookie != null && request.Cookie.Count > 0)
            {
                var c = new CookieContainer();
                foreach (var cItem in request.Cookie)
                {
                    c.Add(new Cookie
                    {
                        Value = cItem.Value,
                        Name = cItem.Name,
                        Domain = cItem.Domain,
                    });
                }
                handler.UseCookies = true;
                handler.CookieContainer = c;
            }
            else
            {
                handler.UseCookies = false;
            }
            #endregion
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            #region RequestHeaders
            if (request.Headers != null && request.Headers.Count > 0)
            {
                foreach (var item in request.Headers)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
            #endregion
            HttpResponseMessage response = null;
            if (request.Method == MethodEnum.GET)
            {
                #region get
                if (request.ContentType == ContentTypeEnum.Byte)
                {
                    HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, request.Url);
                    response = client.SendAsync(req, HttpCompletionOption.ResponseContentRead).Result;
                }
                else
                {
                    response = client.GetAsync(request.Url).Result;
                }
                #endregion
            }
            else
            {
                #region post
                if (request.ContentType == ContentTypeEnum.Form)
                {
                    FormUrlEncodedContent content = new FormUrlEncodedContent(request.BodyDic);
                    response = client.PostAsync(request.Url, content).Result;
                }
                else
                {
                    StringContent strContent = new StringContent(request.Body);
                    response = client.PostAsync(request.Url, strContent).Result;
                }
                #endregion
            }
            if (response.StatusCode == HttpStatusCode.OK)
            {
                try
                {
                    if (request.ContentType == ContentTypeEnum.Byte)
                    {
                        var obj = await response.Content.ReadAsByteArrayAsync();
                        return Ok(new
                        {
                            StatusCode = (int)response.StatusCode,
                            Msg = "",
                            Obj = obj,
                        });
                    }
                    else
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        return Ok(new
                        {
                            StatusCode = (int)response.StatusCode,
                            Msg = "",
                            Data = data,
                        });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            }
            else
            {
                return Ok(new
                {
                    StatusCode = (int)response.StatusCode,
                    Data = response.ToString(),
                });
            }
        }

        /// <summary>
        /// 二维码生成
        /// </summary>
        /// <param name="url"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        [Route("QRCode"), HttpGet]
        public IActionResult QRCodeClient(string url, int w = 300, int h = 300)
        {
            // 编码解码
            url = System.Web.HttpUtility.UrlDecode(url);
            QRCodeGenerator qrG = new QRCodeGenerator();
            QRCodeData qrData = qrG.CreateQrCode(url, QRCodeGenerator.ECCLevel.M, true);
            QRCoder.QRCode qrCode = new QRCoder.QRCode(qrData);
            Bitmap qrImage = qrCode.GetGraphic(5, Color.Black, Color.White, true);
            MemoryStream ms = new MemoryStream();
            qrImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            return File(ms.ToArray(), "image/png");
        }

        #region 私有方法
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        [NonAction]
        private string MD5(string s, Encoding e, bool upper = false)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            if (upper)
            {
                return BitConverter.ToString(md5.ComputeHash(e.GetBytes(s))).Replace("-", string.Empty).ToUpper();
            }
            else
            {
                return BitConverter.ToString(md5.ComputeHash(e.GetBytes(s))).Replace("-", string.Empty).ToLower();
            }
        }
        #endregion

    }
}
