using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificationCode
{
    /// <summary>
    /// Login1 的摘要说明
    /// </summary>
    public class Login1 : IHttpHandler
    {
        private VerificationCodeAESHelp _verificationCodeAESHelp;
        public Login1() { }
        public Login1(VerificationCodeAESHelp verificationCodeAESHelp)
        {
            this._verificationCodeAESHelp = verificationCodeAESHelp;
        }
        public void ProcessRequest(HttpContext context)
        {
            ResponseModel _res = new ResponseModel();

            var userName = context.Request["userName"];
            var passWord = context.Request["passWord"];
            VerifyModel _v = VerifyValiate(context);

            if (!_v.status)
            {
                _res.msg = "重试";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                return;
            }
            if (userName == "admin" && passWord == "admin")
            {
                _res.status = "ok";
                _res.msg = "登陆成功";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                return;
            }
            else
            {
                _res.msg = "账号密码错误";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                return;
            }
        }

        private void SlideVerifyCode( Login1 _Login,bool _bool = false)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new SlideVerifyCodeModel() { SlideCode = _bool });
            string base64Str = _Login._verificationCodeAESHelp.AES_Encrypt_Return_Base64String(json);
            this._verificationCodeAESHelp.SetCookie(VerificationCodeAESHelp._SlideCode, base64Str, 10);

        }
        private VerifyModel VerifyValiate(HttpContext context)
        {

            VerifyModel _v = new VerifyModel();
            VerificationCodeAESHelp _VerificationCodeAESHelp = new VerificationCodeAESHelp(context);
            Login1 _Login = new Login1(_VerificationCodeAESHelp);

            try
            {
                var _cookie = _Login._verificationCodeAESHelp.GetCookie(VerificationCodeAESHelp._SlideCode);
                if (string.IsNullOrEmpty(_cookie))
                {
                    SlideVerifyCode(_Login,false);
                    _v.msg = "请拖动滑块";
                    _v.status = false;
                    return _v;
                }
                _cookie = _cookie.Replace("%", "").Replace(",", "").Replace(" ", "+");
                string _str = _Login._verificationCodeAESHelp.AES_Decrypt_Return_String(_cookie);
                var sildeCodeModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SlideVerifyCodeModel>(_str);

                if (!sildeCodeModel.SlideCode)
                {
                    _v.msg = "请拖动滑块后点击汉字";
                    _v.status = false;
                    return _v;
                }

                var _NowTime = DateTime.Now;
                var _time = sildeCodeModel.timestamp;
                var number = (_NowTime - _time).Minutes;
                if (number > 5)
                {
                    SlideVerifyCode(_Login, false);
                    _v.msg = "滑块验证码过期";
                    _v.status = false;
                    return _v;
                }
            }
            catch (Exception)
            {
                _v.msg = "滑动验证码失败!";
                _v.status = false;
            }

            _v.status = true;
            return _v;

        }

        public class VerifyModel
        {
            public bool status { get; set; }
            public string msg { get; set; }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}