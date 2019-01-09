﻿using Fly.BLL;
using Fly.DomainModel;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Fly.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/FlyAuth")]
    public class FlyAuthController : BaseApiController
    {
        public IHttpActionResult GetToken(string url, string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                return BadRequest(Fly.Resources.OperationLP.InvalidUserNamePassword);
            }

            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "Password",Decrypt(password))
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            var authorizationHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("rajeev:" + password));


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authorizationHeader);
                var tokenResponse = client.PostAsync(url + "Token", new FormUrlEncodedContent(pairs)).Result;
                var token = tokenResponse.Content.ReadAsAsync<Token>(new[] { new JsonMediaTypeFormatter() }).Result;

                return Ok(token);
                
                //var response = client.PostAsync(url + "Token", content).Result;
                //return response.Content.ReadAsStringAsync().Result;
            }
        }

        public HttpResponseMessage Register(RegisterModel regModel)
        {
            using (SecurityUserRepository  secRepository=new SecurityUserRepository())
            {
                reqResponse= secRepository.AddUpdate(new SecurityUser()
                {
                    DeviceId=regModel.device_unique_id,
                    Telephone=regModel.phone_number,
                    Email=regModel.email,
                    Password=regModel.password,
                    FullName=regModel.first_name + " " + regModel.last_name,
                    Gender=regModel.gender,
                    BirthDate=regModel.date_of_birth
                });
            }
            HttpResponseMessage response = Request.CreateResponse(reqResponse.StatusCode, FillMessages(reqResponse.Messages));
            response.Content = new StringContent(FillMessages(reqResponse.Messages), Encoding.Unicode);
           
            
            return response;
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
    }
}
