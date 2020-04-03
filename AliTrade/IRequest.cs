using System;
using System.Collections.Generic;

namespace AliTrade
{
    #region
    public class TaeDictionary : Dictionary<string, string>
    {
        public TaeDictionary() { }
        public TaeDictionary(IDictionary<string, string> dictionary)
            : base(dictionary) { }

        /// <summary>
        /// 添加一个新的键值对。空键或者空值的键值对将会被忽略。
        /// </summary>
        /// <param name="key">键名称</param>
        /// <param name="value">键对应的值，目前支持：string, int, long, double, bool, DateTime类型</param>
        public void Add(string key, object value)
        {
            string strValue;

            if (value == null)
            {
                strValue = null;
            }
            else if (value is string)
            {
                strValue = (string)value;
            }
            else if (value is Nullable<DateTime>)
            {
                Nullable<DateTime> dateTime = value as Nullable<DateTime>;
                strValue = dateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if (value is Nullable<int>)
            {
                strValue = (value as Nullable<int>).Value.ToString();
            }
            else if (value is Nullable<long>)
            {
                strValue = (value as Nullable<long>).Value.ToString();
            }
            else if (value is Nullable<double>)
            {
                strValue = (value as Nullable<double>).Value.ToString();
            }
            else if (value is Nullable<bool>)
            {
                strValue = (value as Nullable<bool>).Value.ToString().ToLower();
            }
            else
            {
                strValue = value.ToString();
            }

            this.Add(key, strValue);
        }
        public new void Add(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                base[key] = value;
            }
        }
    }
    public interface ITaeRequest<out T> where T : TaeResponse
    {
        string GetApiName();
        IDictionary<string, string> GetParameters();
        bool taeReq();
    }
    public abstract class BaseTaeRequest<T> : ITaeRequest<T> where T : TaeResponse
    {
        public string app_key { get; set; }
        public string app_secret { get; set; }
        public abstract string GetApiName();
        public abstract IDictionary<string, string> GetParameters();
        public abstract bool taeReq();
    }
    #endregion

    partial class TbkOrderDetailsQueryRequest : BaseTaeRequest<TbkOrderDetailsQueryResponse>
    {
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public int tk_status { get; set; }
        public int query_type { get; set; }
        public int member_type { get; set; }
        public int order_scene { get; set; }
        public int jump_type { get; set; }
        public string position_index { get; set; }
        public int page_no { get; set; }
        public int page_size { get; set; }

        public override string GetApiName()
        {
            return "taobao.tbk.order.details.get";
        }
        public override IDictionary<string, string> GetParameters()
        {
            TaeDictionary parameters = new TaeDictionary();
            parameters.Add("start_time", this.start_time.ToString("yyyy-MM-dd HH:mm:ss"));
            parameters.Add("end_time", this.end_time.ToString("yyyy-MM-dd HH:mm:ss"));
            if (this.tk_status > 0)
            {
                parameters.Add("tk_status", this.tk_status.ToString());
            }
            parameters.Add("query_type", this.query_type.ToString());
            if (this.member_type > 0)
            {
                parameters.Add("member_type", this.member_type.ToString());
            }
            parameters.Add("order_scene", this.order_scene.ToString());
            parameters.Add("jump_type", this.jump_type.ToString());
            parameters.Add("position_index", this.position_index.ToString());
            parameters.Add("page_no", this.page_no.ToString());
            parameters.Add("page_size", this.page_size.ToString());
            return parameters;
        }
        public override bool taeReq()
        {
            return true;
        }
    }

}
