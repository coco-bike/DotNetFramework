using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MvcExtension;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ThirdParty.MyMVCWeixin;
using ThirdParty.MyMVCWeixin.Common;
using ThirdParty.SenParcWeiXin;

namespace DotNetFramework.Controllers
{
    public class WeiXinController : Controller
    {
        //测试号
        public static readonly string Token = ConfigurationManager.AppSettings["Token"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = ConfigurationManager.AppSettings["EncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = ConfigurationManager.AppSettings["APPID"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string AppSecret = ConfigurationManager.AppSettings["APPSECRET"];
        /// <summary>
        /// 平台验证
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("CheckWechat")]
        public ActionResult Get(string signature, string timestamp, string nonce, string echostr)
        {
            var token = "hello2018";
            if (Utils.CheckSignature(signature, timestamp, nonce, token, out string ent) && !string.IsNullOrEmpty(echostr))
            {
                return Content(echostr);
            }
            return Content("failed:" + signature + "," + "error");
        }
        /// <summary>
        /// 接收消息并发送消息
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CheckWechat")]
        public ActionResult Post(string signature, string timestamp, string nonce, string echostr)
        {
            string postString = string.Empty;
            string contentText = "";
            using (Stream stream = HttpContext.Request.InputStream)
            {
                Byte[] postBytes = new Byte[stream.Length];
                stream.Read(postBytes, 0, (Int32)stream.Length);
                postString = Encoding.UTF8.GetString(postBytes);
                contentText = BaseService.ReviceXml(postString);
            }
            return Content(contentText);
        }
        /// <summary>
        /// Senparc的验证服务器方法
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <param name="echostr"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Index")]
        public ActionResult GetWeChat(string signature, string timestamp, string nonce, string echostr)
        {
            if (CheckSignature.Check(signature, timestamp, nonce, Token))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content("failed:" + signature + "," + CheckSignature.GetSignature(timestamp, nonce, Token) + "。如果您在浏览器中看到这条信息，表明此Url可以填入微信后台。");
            }
        }

        /// <summary>
        /// Senparc的方法
        /// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        /// PS：此方法为简化方法，效果与OldPost一致。
        /// v0.8之后的版本可以结合Senparc.Weixin.MP.MvcExtension扩展包，使用WeixinResult，见MiniPost方法。
        /// </summary>
        [HttpPost]
        [ActionName("Index")]
        public ActionResult PostWeChat(PostModel postModel)
        {
            if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
            {
                return Content("参数错误！");
            }

            postModel.Token = Token;//根据自己后台的设置保持一致
            postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            postModel.AppId = AppId;//根据自己后台的设置保持一致

            //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
            var messageHandler = new CustomMessageHandler(Request.InputStream, postModel);//接收消息

            messageHandler.Execute();//执行微信处理过程

            return new FixWeixinBugWeixinResult(messageHandler);//返回结果

        }
    }
}