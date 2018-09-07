using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VerificationCode
{
    /// <summary>
    /// CheckCode 的摘要说明
    /// </summary>
    public class CheckCode : IHttpHandler
    {

        private VerificationCodeAESHelp _verificationCodeAESHelp;
        public CheckCode() { }
        public CheckCode(VerificationCodeAESHelp verificationCodeAESHelp)
        {
            this._verificationCodeAESHelp = verificationCodeAESHelp;
        }


        public void ProcessRequest(HttpContext context)
        {
            ResponseModel _res = new ResponseModel();

            try
            {
                var code = context.Request["code"];
                var pointList = new List<Point_X_Y>();
                try
                {
                    pointList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Point_X_Y>>(code);
                }
                catch (Exception ex)
                {
                    _res.msg = "验证失败";
                    //context.Response.Write("验证失败");
                    context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                    return;
                }

                if (pointList.Count != 2)
                {
                    _res.msg = "验证失败";
                    context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                    return;
                }
                VerificationCodeAESHelp _V = new VerificationCodeAESHelp(context);
                CheckCode _c = new CheckCode(_V);
                var _cookie = _c._verificationCodeAESHelp.GetCookie(VerificationCodeAESHelp._YZM);

                if (string.IsNullOrEmpty(_cookie))
                {
                    _res.msg = "验证失败";
                    context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                    return;
                }

                _cookie = _cookie.Replace("%", "").Replace(",", "").Replace(" ", "+");
                string _str = _c._verificationCodeAESHelp.AES_Decrypt_Return_String(_cookie);

                var _cookiesPointList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Point_X_Y>>(_str);
                _cookiesPointList = _cookiesPointList.OrderBy(x => x.Sort).ToList();
                int i = 0;
                foreach (var item in pointList.AsParallel())
                {
                    int _x = _cookiesPointList[i]._X - item._X;
                    int _y = _cookiesPointList[i]._Y - item._Y;
                    _x = Math.Abs(_x);
                    _y = Math.Abs(_y);
                    if (_x > 25 || _y > 25)
                    {
                        _res.msg = "验证失败";
                        context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                        return;
                    }
                    i++;
                }
                SlideVerifyCode(_c,true);

            }
            catch (Exception)
            {
                _res.msg = "验证失败";
                context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
                return;
            }

            _res.status = "ok";
            _res.msg = "验证通过";
           context.Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(_res));
            return;

            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
        }

        public class ResponseModel
        {
            public string status { get; set; }
            public string msg { get; set; }
        }

        private void SlideVerifyCode(CheckCode _c,bool _bool = false)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new SlideVerifyCodeModel() { SlideCode = _bool });
            //滑块验证先通过；
            string base64Str = _c._verificationCodeAESHelp.AES_Encrypt_Return_Base64String(json);
            _c._verificationCodeAESHelp.SetCookie(VerificationCodeAESHelp._SlideCode, base64Str, 10);
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