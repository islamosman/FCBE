﻿using Fly.BLL;
using Fly.DomainModel;
using Fly.Providers;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Fly.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/FlyAuth")]
    public class FlyAuthController : BaseApiController
    {
        public log4net.ILog logger;
        public FlyAuthController()
        {
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        [HttpPost]
        [Route("upload")]
        public HttpResponseMessage uploadImage()
        {
            var request = HttpContext.Current.Request;

            if (Request.Content.IsMimeMultipartContent())
            {
                if (request.Files.Count > 0)
                {
                    var postedFile = request.Files.Get("file");
                    var title = request.Params["title"];
                    string root = HttpContext.Current.Server.MapPath("~/Images");
                    root = root + "/" + postedFile.FileName;
                    postedFile.SaveAs(root);
                    //Save post to DB
                    return Request.CreateResponse(HttpStatusCode.Found, new
                    {
                        error = false,
                        status = "created",
                        path = root
                    });

                }
            }

            return null;
        }

        public IHttpActionResult Login(LoginModel loginModel)
        {

            try
            {
                if (string.IsNullOrEmpty(loginModel.userName) || string.IsNullOrEmpty(loginModel.password))
                {
                    return BadRequest(Fly.Resources.OperationLP.InvalidUserNamePassword);
                }

                loginModel.password =WebUI.Helpers.WebUiUtility.Encrypt(loginModel.password);
                var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", loginModel.userName ),
                        new KeyValuePair<string, string> ( "Password",loginModel.password)
                    };
                var content = new FormUrlEncodedContent(pairs);
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("rajeev:" + loginModel.password));


                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; // comparable to modern browsers

                using (var client = new HttpClient())
                {
                
                var response = client.PostAsync(new Uri(System.Configuration.ConfigurationManager.AppSettings["ServiceUrl"].ToString() + "Token"), content).Result;
                
                var token = response.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;
                    using (SecurityUserRepository obj = new SecurityUserRepository())
                    {
                        SecurityUser secUserModel = obj.GetBy(loginModel.userName, loginModel.password);
                        if (secUserModel != null)
                        {
                            token.UserId = secUserModel.PayMobSendId;
                            token.Tocken = secUserModel.TockenToP;
                            token.UserName = secUserModel.FullName;
                        }
                    }
                    // var sss = response.Content.ReadAsStringAsync().Result;
                    //return Json(new { tock = sss });
                    return Ok(token);
                }
            }
            catch (OperationCanceledException oce)
            {
                logger.Error(oce.Message + " < " + oce.InnerException.Message + " < " + oce.StackTrace + " == " + oce.Data);
                return Ok(new { success = false, access_token = "" });
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message + " > " + ex.InnerException.Message + " > " + ex.StackTrace);
                return Ok(new { success = false, access_token = "" });
            }

        }

        [Route("Register")]
        public IHttpActionResult Register(RegisterModel regModel)
        {
            using (SecurityUserRepository secRepository = new SecurityUserRepository())
            {
                reqResponse = secRepository.AddUpdate(new SecurityUser()
                {
                    DeviceId = regModel.device_unique_id,
                    Telephone = regModel.phone_number,
                    Email = regModel.email.Trim(),
                    Password = WebUI.Helpers.WebUiUtility.Encrypt(regModel.password.Trim()),
                    FullName = regModel.first_name + " " + regModel.last_name,
                    Gender = regModel.gender,
                    BirthDate = regModel.date_of_birth,
                    IsActive = false
                });

                if (reqResponse.IsDone)
                {
                    SMTPEmailSender EmailProxy = new SMTPEmailSender();

                    string msgBody = Resources.OperationLP.subscribtionMail.Replace("{0}", reqResponse.ResponseIdStr);
                    //string msgBody = "Activation Code : " + reqResponse.ResponseIdStr;
                    //string msgBody = string.Format(Resources.OperationLP.subscribtionMail ,reqResponse.ResponseIdStr);

                    EmailProxy.SendEmail("", regModel.email, "Subscription activation", msgBody, true);
                }
            }
            return Json(reqResponse);
        }

        [Route("VerfiyPass")]
        public IHttpActionResult VerfiyPass(VerifyPassCodeModel verifyModel)
        {
            using (SecurityUserRepository secRepository = new SecurityUserRepository())
            {
                reqResponse = secRepository.VerfiyPassCode(verifyModel);
            }
            return Json(reqResponse);
        }
        //[HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Authorize(Roles = "Supervisor")]
        [Route("UserInfo")]
        public string GetUserInfo()
        {
            return "Supervisor";
        }

        [Authorize(Roles = "Admin")]
        [Route("UserInfo2")]
        public string GetUserInfoAdmin()
        {
            return "Admin";
        }


        //public  IHttpActionResult RefreshToken(string url,string refreshToken)
        //{

        //var pairs = new List<KeyValuePair<string, string>>
        //        {
        //            new KeyValuePair<string, string>( "grant_type", "refresh_token" ),
        //            new KeyValuePair<string, string>( "refresh_token", refreshToken ),
        //        };
        //var content = new FormUrlEncodedContent(pairs);
        //ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        //using (var client = new HttpClient())
        //{
        //    var response = client.pos(url + "Token", content).Result;
        //    return Ok(response.Content.ReadAsStringAsync().Result);
        //}

        //    var client = new HttpClient()
        //         client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
        //    var tokenResponse = client..PostAsync(url + "Token", new FormUrlEncodedContent(pairs)).Result;
        //    //    var client = new OAuth2Client(

        //    //        new Uri("http://localhost:2727/token "),

        //    //"client1",

        //    //"secret");



        //    var response = client.RequestRefreshTokenAsync(refreshToken).Result;

        //    return Ok(response);

        // }
    }

    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        public string UserId { get; set; }
        public string Tocken { get; set; }

        public string UserName { get; set; }
    }
}
