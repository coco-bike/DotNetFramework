using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
    /// <summary>
    /// URL的操作类
    /// </summary>
    public class UrlOper
    {
        static System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        #region URL的64位编码
        /// <summary>
        /// URL的64位编码
        /// </summary>
        /// <param name="sourthUrl"></param>
        /// <returns></returns>
        public static string Base64Encrypt(string sourthUrl)
        {
            //string eurl = HttpUtility.UrlEncode(encoding.GetBytes(sourthUrl));
            byte[] byteJson = encoding.GetBytes(sourthUrl);
            string eurl = Convert.ToBase64String(byteJson);
            return eurl;
        }
        /// <summary>
        /// UTF-8编码
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static string UrlEncode_UTF8(string eStr)
        {
            return HttpUtility.UrlEncode(eStr, System.Text.Encoding.UTF8);
        }

        public static string UrlDecrypt_UTF8(string eStr)
        {
            return HttpUtility.UrlDecode(eStr, Encoding.UTF8);
        }
        #endregion

        #region URL的64位解码
        public static string Base64Decrypt(string eStr)
        {
            if (!IsBase64(eStr))
            {
                return eStr;
            }
            byte[] buffer = Convert.FromBase64String(eStr);
            string sourthUrl = encoding.GetString(buffer);
            sourthUrl = HttpUtility.UrlDecode(sourthUrl);
            return sourthUrl;
        }
        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(string eStr)
        {
            if ((eStr.Length % 4) != 0)
            {
                return false;
            }
            if (!Regex.IsMatch(eStr, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 添加URL参数
        /// </summary>
        public static string AddParam(string url, string paramName, string value)
        {
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.Query))
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "?" + paramName + "=" + eval);
            }
            else
            {
                string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "&" + paramName + "=" + eval);
            }
        }
        /// <summary>
        /// 更新URL参数
        /// </summary>
        public static string UpdateParam(string url, string paramName, string value)
        {
            string keyWord = paramName + "=";
            int index = url.IndexOf(keyWord) + keyWord.Length;
            int index1 = url.IndexOf("&", index);
            if (index1 == -1)
            {
                url = url.Remove(index, url.Length - index);
                url = string.Concat(url, value);
                return url;
            }
            url = url.Remove(index, index1 - index);
            url = url.Insert(index, value);
            return url;
        }

        #region 分析URL所属的域
        public static void GetDomain(string fromUrl, out string domain, out string subDomain)
        {
            domain = "";
            subDomain = "";
            try
            {
                if (fromUrl.IndexOf("的名片") > -1)
                {
                    subDomain = fromUrl;
                    domain = "名片";
                    return;
                }

                UriBuilder builder = new UriBuilder(fromUrl);
                fromUrl = builder.ToString();

                Uri u = new Uri(fromUrl);

                if (u.IsWellFormedOriginalString())
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";

                    }
                    else
                    {
                        string Authority = u.Authority;
                        string[] ss = u.Authority.Split('.');
                        if (ss.Length == 2)
                        {
                            Authority = "www." + Authority;
                        }
                        int index = Authority.IndexOf('.', 0);
                        domain = Authority.Substring(index + 1, Authority.Length - index - 1).Replace("comhttp", "com");
                        subDomain = Authority.Replace("comhttp", "com");
                        if (ss.Length < 2)
                        {
                            domain = "不明路径";
                            subDomain = "不明路径";
                        }
                    }
                }
                else
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";
                    }
                    else
                    {
                        subDomain = domain = "不明路径";
                    }
                }
            }
            catch
            {
                subDomain = domain = "不明路径";
            }
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            nvc = new NameValueCollection();
            baseUrl = "";

            if (url == "")
                return;

            int questionMarkIndex = url.IndexOf('?');

            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);

            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);

            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }

        #endregion

        /// <summary>  
        /// 获取客户端Ip  
        /// </summary>  
        /// <returns></returns>
        public static string GetClientIp()
        {
            string clientIP = "";
            if (System.Web.HttpContext.Current != null)
            {
                clientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(clientIP) || (clientIP.ToLower() == "unknown"))
                {
                    clientIP = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_REAL_IP"];
                    if (string.IsNullOrEmpty(clientIP))
                    {
                        clientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                else
                {
                    clientIP = clientIP.Split(',')[0];
                }
                //try
                //{
                //    Common.LogHelper.WritePathReport("Request", "【" + clientIP + "】【获取浏览器】" + GetBrowser()
                //        + "【操作系统版本号】" + SystemCheck());
                //}
                //catch(Exception ex)
                //{
                //    Common.LogHelper.WritePathReport("Request","【"+ clientIP + "】"+ex.Message);
                //}
            }
            return clientIP;
        }

        /// <summary>
        /// 获取浏览器+版本号
        /// </summary>
        /// <returns></returns>
        public static string GetBrowser()
        {
            try
            {
                string browsers;
                HttpBrowserCapabilities bc = HttpContext.Current.Request.Browser;
                string aa = bc.Browser.ToString();
                string bb = bc.Version.ToString();
                browsers = aa + bb;
                return browsers;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return "";
            }
        }

        /// <summary>
        /// 获取操作系统版本号  
        /// </summary>
        /// <returns></returns>
        public static string SystemCheck()
        {
            try
            {
                string Agent = HttpContext.Current.Request.ServerVariables["HTTP_USER_AGENT"];
                if (Agent.IndexOf("NT 4.0") > 0)
                    return "Windows NT ";
                else if (Agent.IndexOf("NT 5.0") > 0)
                    return "Windows 2000";
                else if (Agent.IndexOf("NT 5.1") > 0)
                    return "Windows XP";
                else if (Agent.IndexOf("NT 5.2") > 0)
                    return "Windows 2003";
                else if (Agent.IndexOf("NT 6.0") > 0)
                    return "Windows Vista";
                else if (Agent.IndexOf("NT 7.0") > 0)
                    return "Windows 7";
                else if (Agent.IndexOf("NT 8.0") > 0)
                    return "Windows 8";
                else if (Agent.IndexOf("NT 10.0") > 0)
                    return "Windows 10";
                else if (Agent.IndexOf("WindowsCE") > 0)
                    return "Windows CE";
                else if (Agent.IndexOf("NT") > 0)
                    return "Windows NT ";
                else if (Agent.IndexOf("9x") > 0)
                    return "Windows ME";
                else if (Agent.IndexOf("98") > 0)
                    return "Windows 98";
                else if (Agent.IndexOf("95") > 0)
                    return "Windows 95";
                else if (Agent.IndexOf("Win32") > 0)
                    return "Win32";
                else if (Agent.IndexOf("Linux") > 0)
                    return "Linux";
                else if (Agent.IndexOf("SunOS") > 0)
                    return "SunOS";
                else if (Agent.IndexOf("Mac") > 0)
                    return "Mac";
                else if (Agent.IndexOf("Linux") > 0)
                    return "Linux";
                else if (Agent.IndexOf("Windows") > 0)
                    return "Windows";
                return "未知类型";

            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex);
                return "";
            }

        }

    }
}
