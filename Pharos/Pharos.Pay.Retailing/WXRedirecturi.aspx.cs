﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Pharos.Component.qrcode;
using Pharos.Component.qrcode.wx;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Pharos.Pay.Retailing
{
    //微信访问H5授权页面
    public partial class WXRedirecturi : System.Web.UI.Page
    {
        //菜单配置路径https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxbeb2e5a46ca7bf69&redirect_uri=http%3a%2f%2fdemo.xmpharos.com%2fWXRedirecturi.aspx%3fuserid%3d123&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect
        protected void Page_Load(object sender, EventArgs e)
        {
            Log.Debug(this.GetType().Name, "进入微信授权页面");
            var code = Request["code"];
            Log.Debug(this.GetType().Name, code);
            Label1.Text = code;
            if(!string.IsNullOrWhiteSpace(code))
            {
                string appid = "wxbeb2e5a46ca7bf69", secret = "a7c06194b79c478415d9ddce62b7a244";
                //超过3个月未使用appsecret,会变换,与商户API安全KEY不同一个
                //网页授权access_token
                var getTokenUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?appid="+appid+"&secret="+secret+"&code=" + code + "&grant_type=authorization_code";
                string response = HttpService.Get(getTokenUrl);
                Log.Debug(getTokenUrl, "get access_token : " + response);
                try
                {
                    var json = JObject.Parse(response);

                    var token = Convert.ToString(json.Property("access_token").Value);
                    var openid = Convert.ToString(json.Property("openid").Value);
                    if (!string.IsNullOrWhiteSpace(token) && !string.IsNullOrWhiteSpace(openid))
                    {
                        var getUserUrl = "https://api.weixin.qq.com/sns/userinfo?access_token=" + token + "&openid=" + openid + "&lang=zh_CN";
                        response = HttpService.Get(getUserUrl);
                        Log.Debug(getUserUrl, "get user info : " + response);

                        //普通access_token
                        var getAccessToken = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid="+appid+"&secret="+secret;
                        response = HttpService.Get(getAccessToken);
                        Log.Debug(getAccessToken, "get AccessToken info : " + response);
                        json = JObject.Parse(response);
                        //发送消息
                        var obj = new { touser = openid, msgtype = "text", text = new {
                            content="hello,你访问了请购单!"
                        } };
                        var pushUrl = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" +Convert.ToString(json["access_token"]);
                        //response= HttpService.Post(Newtonsoft.Json.JsonConvert.SerializeObject(obj), pushUrl, false, 10, contentType: "text/json");
                        //Log.Debug(pushUrl, "get push info : " + response);
                    }
                }
                catch { }
                Label2.Text = response;
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/jsapi/PayFor.aspx");
        }
        protected void LinkButton2_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/jsapi2/PayFor2.aspx");
        }
    }
}