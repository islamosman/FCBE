﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.BLL;
using Fly.DomainModel;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Dynamic;
using System.Collections;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using RestSharp;
using Fly.WebUI.Helpers;

namespace JsonUtils
{
    /// <summary>
    /// Creates a dynamic object
    /// Methods that can be used on arrays: foreach, ToArray(), ToList(), Count, Length
    /// </summary>
    public class JsonObject : DynamicObject, IEnumerable, IEnumerator
    {
        object _object;

        JsonObject(object jObject)
        {
            this._object = jObject;
        }

        public static dynamic GetDynamicJsonObject(byte[] buf)
        {
            return GetDynamicJsonObject(buf, Encoding.UTF8);
        }

        public static dynamic GetDynamicJsonObject(byte[] buf, Encoding encoding)
        {
            return GetDynamicJsonObject(encoding.GetString(buf));
        }

        public static dynamic GetDynamicJsonObject(string json)
        {
            object o = JsonConvert.DeserializeObject(json);
            return new JsonUtils.JsonObject(o);
        }

        internal static dynamic GetDynamicJsonObject(JObject jObj)
        {
            return new JsonUtils.JsonObject(jObj);
        }

        public object this[string s]
        {
            get
            {
                JObject jObject = _object as JObject;
                object obj = jObject.SelectToken(s);
                if (obj == null) return true;

                if (obj is JValue)
                    return GetValue(obj);
                else
                    return new JsonObject(obj);
            }
        }

        public object this[int i]
        {
            get
            {
                if (!(_object is JArray)) return null;

                object obj = (_object as JArray)[i];
                if (obj is JValue)
                {
                    return GetValue(obj);
                }
                return new JsonObject(obj);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;

            if (_object is JArray)
            {
                JArray jArray = _object as JArray;
                switch (binder.Name)
                {
                    case "Length":
                    case "Count": result = jArray.Count; break;
                    case "ToList": result = (Func<List<string>>)(() => jArray.Values().Select(x => x.ToString()).ToList()); break;
                    case "ToArray": result = (Func<string[]>)(() => jArray.Values().Select(x => x.ToString()).ToArray()); break;
                }

                return true;
            }

            JObject jObject = _object as JObject;
            object obj = jObject.SelectToken(binder.Name);
            if (obj == null) return true;

            if (obj is JValue)
                result = GetValue(obj);
            else
                result = new JsonObject(obj);

            return true;
        }

        object GetValue(object obj)
        {
            string val = ((JValue)obj).ToString();

            int resInt; double resDouble; DateTime resDateTime;

            if (int.TryParse(val, out resInt)) return resInt;
            if (DateTime.TryParse(val, out resDateTime)) return resDateTime;
            if (double.TryParse(val, out resDouble)) return resDouble;

            return val;
        }

        public override string ToString()
        {
            return _object.ToString();
        }

        int _index = -1;

        public IEnumerator GetEnumerator()
        {
            _index = -1;
            return this;
        }

        public object Current
        {
            get
            {
                if (!(_object is JArray)) return null;
                object obj = (_object as JArray)[_index];
                if (obj is JValue) return GetValue(obj);
                return new JsonObject(obj);
            }
        }

        public bool MoveNext()
        {
            if (!(_object is JArray)) return false;
            _index++;
            return _index < (_object as JArray).Count;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
namespace Fly.Controllers
{
    public static class JsonExtensions
    {
        public static JsonUtils.JsonObject GetDynamicJsonObject(this Uri uri)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Encoding = System.Text.Encoding.UTF8;
                wc.Headers["User-Agent"] = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET4.0C; .NET4.0E)";
                return JsonUtils.JsonObject.GetDynamicJsonObject(wc.DownloadString(uri.ToString()));
            }
        }
    }

    public class HomeController : AdminBaseController
    {
        public ActionResult Index()
        {
            //var sade = WebUiUtility.Encrypt1("Hesham@1996");
            //var dd2=WebUiUtility.Decrypt("/jP53y+1Br2iPaxKNhFsSk7NA2fiR2t32ejC6XTraaQvLRe62Xc28XaMSthtg14J5YIS96PE0gPcFIM0up+wIA==");
            //var fft23 = WebUiUtility.Decrypt("+WPIc7vw353xO323FDmap+gJWo/5P9hwPTnuc/+GNXtFjLaypbCsGZVabrfKdAjbLf02+hrCKMIDniAXk4t9bQ==");

            // TestAPI();//.Wait();
            Gmap.net.GoogleMapApi dd = new Gmap.net.GoogleMapApi(false);
            Gmap.net.Overlays.Polygon ssw = new Gmap.net.Overlays.Polygon("D");
            ssw.Points.Add(new Gmap.net.Location() { Latitude = 30.042287586068877, Longitude = 31.166267037884495 });
            ssw.Points.Add(new Gmap.net.Location() { Latitude = 30.042287586068877, Longitude = 31.224159837261936 });
            ssw.Points.Add(new Gmap.net.Location() { Latitude = 30.017877075175274, Longitude = 31.166267037884495 });
            ssw.Points.Add(new Gmap.net.Location() { Latitude = 30.017877075175274, Longitude = 31.224159837261936 });
            ssw.Points.Add(new Gmap.net.Location() { Latitude = 30.042287586068877, Longitude = 31.224159837261936 });
            ssw.Points.Add(new Gmap.net.Location() { Latitude = 30.017877075175274, Longitude = 31.224159837261936 });
            dd.CallJs();
            JavaScriptResult ddd = JavaScript("return '1';");

            
            bool isContain = ssw.Points.Contains(new Gmap.net.Location() { Latitude = 30.022806863746357, Longitude = 31.207265853881836 });


            return View();
        }

        private void TestAPI()
        {
            var dataTo = new
            {
                api_key = "ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SnVZVzFsSWpvaWFXNXBkR2xoYkNJc0luQnliMlpwYkdWZmNHc2lPak15T0RFc0ltTnNZWE56SWpvaVRXVnlZMmhoYm5RaWZRLmt6UHZFTV9pc1RINkZWWVJGWUJ2ZDBIVDFtMWlISXZoRGJ0ckZwaWJDdHF4QmduY2xvb2dUZ3dzMG1FMlJaYmM0MFdBbHJwM0lhRGk2dkZHbW1PblVB"
            };

            var json = JsonConvert.SerializeObject(dataTo);
            var stringContent = new StringContent(json, Encoding.UTF8,
                        "application/json");


            var client = new RestClient("https://accept.paymobsolutions.com/api/auth/tokens");
            // client.Authenticator = new HttpBasicAuthenticator(username, password);

            var request = new RestRequest("resource/{id}", Method.POST);
            request.AddJsonBody(stringContent);
            request.AddParameter("name", "value"); // adds to POST or URL querystring based on Method
            request.AddUrlSegment("id", "123"); // replaces matching token in request.Resource

            // easily add HTTP Headers
            request.AddHeader("header", "value");


            // execute the request
            IRestResponse response = client.Execute(request);
            var content = response.Content; // raw content as string



            using (HttpClient httpClient = new HttpClient())
            {
                var response2 = httpClient.PostAsJsonAsync("https://accept.paymobsolutions.com/api/auth/tokens", stringContent).Result;
                //var response = httpClient.PostAsync("https://accept.paymobsolutions.com/api/auth/tokens","",).GetAwaiter().GetResult();
                //var result = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                // Do stuff...
            }

            //using (var client = new HttpClient())
            //{
            //    var json = JsonConvert.SerializeObject(dataTo);
            //    var stringContent = new StringContent(json, Encoding.UTF8,
            //                "application/json");
            //    var modeldd = await client.PostAsJsonAsync("https://accept.paymobsolutions.com/api/auth/tokens", stringContent);
            //}

            //using (var client = new HttpClient())
            //{
              

            //    HttpResponseMessage response = await client.PostAsJsonAsync("https://accept.paymobsolutions.com/api/auth/tokens", dataTo);

            //    if (response.IsSuccessStatusCode)
            //    {
            //        // Get the URI of the created resource.  
            //        string dd = "";
            //    }
            //}

            //return Json(new { success = "success" }, JsonRequestBehavior.AllowGet);
        }

        public double distance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            if ((lat1 == lat2) && (lon1 == lon2))
            {
                return 0;
            }
            else
            {
                double theta = lon1 - lon2;
                double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
                dist = Math.Acos(dist);
                dist = rad2deg(dist);
                dist = dist * 60 * 1.1515;
                if (unit == 'K')
                {
                    dist = dist * 1.609344;
                }
                else if (unit == 'N')
                {
                    dist = dist * 0.8684;
                }
                return (dist);
            }
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, char unit = 'K')
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }


        [HttpPost]
        public ActionResult GetAllVehicls()
        {
            using (VehicleRepository scoterRepo = new VehicleRepository())
            {
                return Json(scoterRepo.getAllForDash());
            }
        }
    }
}
