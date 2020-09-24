using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnetcoreweb
{
    public class Class
    {
        #region req 方法
        /// <summary>
        /// 请求类接口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public interface IRequest<out T> where T : AResponse
        {
            // 接口名称
            // 业务参数（字典）
            // 密钥参数（字典）
            // 网关
        }
        /// <summary>
        /// 请求类抽象方法，部分实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class ARequest<T> : IRequest<T> where T : AResponse
        {
            // abstract 抽象类需要被实现
            // virtual 虚方法可以被重写
        }
        /// <summary>
        /// 请求类方法：各自实现
        /// </summary>
        public class Request : ARequest<AResponse>
        {
            // 业务参数、方法
            // override 重写抽象方法和虚方法
            // new 覆盖同名方法
        }
        #endregion

        #region rep 方法
        public class ErrResponse
        {
            // code
            // msg
        }
        /// <summary>
        /// 响应类对象：公共
        /// </summary>
        public abstract class AResponse
        {
            // code
            // msg
            // body
            // errrep
            // ErrorBool
        }
        /// <summary>
        /// 响应类对象：各自
        /// </summary>
        public class Response : AResponse
        {
            // result.list.data
            // string
        }
        #endregion

        /// <summary>
        /// 普通数据请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private static T Execute<T>(IRequest<T> request) where T : AResponse
        {
            return null;
        }
        /// <summary>
        /// 证书数据请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private static T CertificateExecute<T>(IRequest<T> request) where T : AResponse
        {
            return null;
        }
        /// <summary>
        /// 文件数据请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private static T FileExecute<T>(IRequest<T> request) where T : AResponse
        {
            return null;
        }

    }
}