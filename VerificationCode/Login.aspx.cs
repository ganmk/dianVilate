using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VerificationCode
{
    public partial class Login : System.Web.UI.Page
    {
        private VerificationCodeAESHelp _verificationCodeAESHelp;
        public Login() { }
        public Login(VerificationCodeAESHelp verificationCodeAESHelp)
        {
            this._verificationCodeAESHelp = verificationCodeAESHelp;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
             
            SlideVerifyCode();
        }

        private void SlideVerifyCode(bool _bool = false)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new SlideVerifyCodeModel() { SlideCode = _bool });
            VerificationCodeAESHelp _VerificationCodeAESHelp=new VerificationCodeAESHelp (HttpContext.Current);

            Login _login = new Login(_VerificationCodeAESHelp);
            string base64Str = _login._verificationCodeAESHelp.AES_Encrypt_Return_Base64String(json);
            _login._verificationCodeAESHelp.SetCookie(VerificationCodeAESHelp._SlideCode, base64Str, 10);

        }
    }
}