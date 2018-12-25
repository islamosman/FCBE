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

namespace Fly.Controllers
{
    [RoutePrefix("api/FlyAuth")]
    public class FlyAuthController : ApiController
    {
        public IHttpActionResult GetToken(string url, string userName, string password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", userName ),
                        new KeyValuePair<string, string> ( "Password", password )
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


        public  IHttpActionResult RefreshToken(string url,string refreshToken)
        {

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

        }
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
