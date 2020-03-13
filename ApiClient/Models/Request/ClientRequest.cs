using System.Collections.Generic;

namespace ApiClient.Controllers.Models.Request
{
    #region tae
    public class TaeRequest
    {
        /// <summary>
        /// 大写特殊情况 对照真实的Body
        /// </summary>
        /// <value></value>
        public Dictionary<string, string> Body { get; set; }
    }
    #endregion

    #region acs
    public enum MethodEnum
    {
        GET,
        POST,
    }
    public enum ContentTypeEnum
    {
        Form,
        String,
        Json,
        Byte,
    }
    public class AcsRequest
    {
        public string Url { get; set; }
        public MethodEnum Method { get; set; }
        public ContentTypeEnum ContentType { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> BodyDic { get; set; }
        public List<CookieTemp> Cookie { get; set; }
        public string BodyJson { get; set; }
        public string Body { get; set; }
    }

    public class CookieTemp
    {
        public string Domain { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
    #endregion
}
