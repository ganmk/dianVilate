using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificationCode
{
    public class PathHelper
    {
        /// <summary>
        /// 绝对路径转相对路径
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string urlConvertor(string strUrl)
        {
            string tmpRootDir = HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string urlPath = strUrl.Replace(tmpRootDir, ""); //转换成相对路径
            urlPath = urlPath.Replace(@"/", @"/");
            return urlPath;
        }

        /// <summary>
        /// 相对路径转绝对路径
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string urlConvertorLocal(string strUrl)
        {
            string tmpRootDir = HttpContext.Current.Server.MapPath(System.Web.HttpContext.Current.Request.ApplicationPath.ToString());//获取程序根目录
            string urlPath = tmpRootDir + strUrl.Replace(@"/", @"/"); //转换成绝对路径
            return urlPath;
        }

    }
}