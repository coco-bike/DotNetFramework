using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ThirdParty.MyMVCWeixin.Common;
using ThirdParty.MyMVCWeixin.Model;

namespace ThirdParty.MyMVCWeixin
{
    public class BaseService
    {
        /// <summary>
        /// 返回Http客户端请求
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static HttpResponseMessage GetReturn(string message)
        {
            return new HttpResponseMessage
            {
                Content = new StringContent(message, Encoding.UTF8, "application/x-www-form-urlencoded")
            };
        }
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <param name="appid">应用ID</param>
        /// <param name="secret">应用密钥</param>
        /// <returns>AccessToken实体</returns>
        public static AccessToken GetAccessToken(string appid, string secret)
        {
            var url =
            string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", appid, secret);
            return Utils.GetResult<AccessToken>(url);
        }

        /// <summary>
        /// 处理信息并应答
        /// </summary>
        public static string ReviceXml(string postStr)
        {
            WeixinApiDispatch help = new WeixinApiDispatch();
            string responseContent = help.ReturnMessage(postStr);
            return responseContent;
        }
    }
}
