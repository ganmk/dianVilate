using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace VerificationCode
{
    /// <summary>
    /// VerificationCodeImage1 的摘要说明
    /// </summary>
    public class VerificationCodeImage1 : HttpTaskAsyncHandler
    {
        private VerificationCodeAESHelp _verificationCodeAESHelp;
        public VerificationCodeImage1() { }
        public VerificationCodeImage1(VerificationCodeAESHelp verificationCodeAESHelp)
        {
            this._verificationCodeAESHelp = verificationCodeAESHelp;
        }

        public override async Task ProcessRequestAsync(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var data = await GetVerificationCodeImage(context);
            context.Response.Write(data);

        }

        public async Task<string> GetVerificationCodeImage(HttpContext context)
        {

            ResponseModel _res=new ResponseModel ();
            try
            {
                var model = await VerificationCode.VerificationCodeImage.CreateHanZi();
                var json_Model = Newtonsoft.Json.JsonConvert.SerializeObject(model.point_X_Y);
                VerificationCodeAESHelp _VerificationCodeAESHelp = new VerificationCodeAESHelp(context);
                VerificationCodeImage1 _v = new VerificationCodeImage1(_VerificationCodeAESHelp);

                string pointBase64str = _v._verificationCodeAESHelp.AES_Encrypt_Return_Base64String(json_Model);
                _v._verificationCodeAESHelp.SetCookie(VerificationCodeAESHelp._YZM, pointBase64str, 5);
                string msg = "请根据顺序点击【" + string.Join("", model.point_X_Y.Select(x => x.Word).ToList()) + "】";
                _res.msg = msg;
                _res.result = model.ImageBase64Str;
            }
            catch (Exception ex)
            {
                  return Newtonsoft.Json.JsonConvert.SerializeObject(ex);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(_res);
        }

        public class ResponseModel
        {
            public string result { get; set; }
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