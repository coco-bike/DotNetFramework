﻿using Senparc.Weixin.Exceptions;
using Senparc.Weixin.HttpUtility;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Menu;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace ThirdParty.SenParcWeiXin.Menu
{
    public class AdminMenu
    {
        public static readonly string Token = ConfigurationManager.AppSettings["Token"];//与微信公众账号后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = ConfigurationManager.AppSettings["EncodingAESKey"];//与微信公众账号后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string AppId = ConfigurationManager.AppSettings["APPID"];//与微信公众账号后台的AppId设置保持一致，区分大小写。
        public static readonly string AppSecret = ConfigurationManager.AppSettings["APPSECRET"];
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <returns></returns>
        public static int CreateMenu()
        {
            var accessToken = AccessTokenContainer.TryGetAccessToken(AppId,AppSecret);
            ButtonGroup bg = new ButtonGroup();

            //单击
            bg.button.Add(new SingleClickButton()
            {
                name = "单击测试",
                key = "OneClick",
                type = ButtonType.click.ToString(),//默认已经设为此类型，这里只作为演示
            });

            //二级菜单
            var subButton = new SubButton()
            {
                name = "二级菜单"
            };
            subButton.sub_button.Add(new SingleClickButton()
            {
                key = "SubClickRoot_Text",
                name = "返回文本"
            });
            subButton.sub_button.Add(new SingleClickButton()
            {
                key = "SubClickRoot_News",
                name = "返回图文"
            });
            subButton.sub_button.Add(new SingleClickButton()
            {
                key = "SubClickRoot_Music",
                name = "返回音乐"
            });
            subButton.sub_button.Add(new SingleViewButton()
            {
                url = "http://weixin.senparc.com",
                name = "Url跳转"
            });
            bg.button.Add(subButton);
            var result = CommonApi.CreateMenu(accessToken, bg);
            return result.ErrorCodeValue;
        }
        /// <summary>
        /// 菜单删除
        /// </summary>
        /// <returns></returns>
        public static int DeleteMenu()
        {
            var accessToken = AccessTokenContainer.TryGetAccessToken(AppId, AppSecret);
            var result = CommonApi.GetMenu(accessToken);
            return result.ErrorCodeValue;
        }

        /// <summary>
        /// 获取当前菜单，如果菜单不存在，将返回null
        /// </summary>
        /// <param name="accessTokenOrAppId">AccessToken或AppId。当为AppId时，如果AccessToken错误将自动获取一次。当为null时，获取当前注册的第一个AppId。</param>
        /// <returns></returns>
        public static GetMenuResult GetMenu(string accessTokenOrAppId)
        {
            return ApiHandlerWapper.TryCommonApi(accessToken =>
            {
                var url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", accessToken);

                var jsonString = RequestUtility.HttpGet(url, Encoding.UTF8);
                //var finalResult = GetMenuFromJson(jsonString);

                GetMenuResult finalResult;
                JavaScriptSerializer js = new JavaScriptSerializer();
                try
                {
                    var jsonResult = js.Deserialize<GetMenuResultFull>(jsonString);
                    if (jsonResult.menu == null || jsonResult.menu.button.Count == 0)
                    {
                        throw new WeixinException(jsonResult.errmsg);
                    }

                    finalResult = GetMenuFromJsonResult(jsonResult);
                }
                catch (WeixinException ex)
                {
                    finalResult = null;
                }

                return finalResult;
            }, accessTokenOrAppId);
        }
    }
}
