using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Fly.BLL;
using Fly.DomainModel.Helper;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;
using System.Web;
using HttpUtils;
using System.Security.Claims;

namespace Fly.Controllers
{
    [RoutePrefix("api/Vehicles")]
    public class VehiclesController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [Route("GetVehicls")]
        [HttpPost]
        public IHttpActionResult GetVehicls()
        {
            var vs = new VehicleRepository().GetAll().ToList();
            return Ok(new { result = vs });
        }

        [Route("VehiclStatus")]
        [HttpPost]
        public IHttpActionResult VehiclStatus(string status)
        {
            var vs = new TempRepository().AddUpdate(new DomainModel.TempStatus()
            {
                CreatedDate = DateTime.Now,
                DataStr = status
            });
            return Ok(new { ss = vs.ResponseIdStr });
        }


        [Authorize(Roles = "User")]
        [Route("UserStatus")]
        [HttpGet]
        public IHttpActionResult UserStatus()
        {
            int userId = GetUserId();
            if (userId == 0)
            {
                reqResponse.ErrorMessages.Add("noUser", "Invalid Data");
                return Ok(reqResponse);
            }
            using (SecurityUserRepository secUserRepo = new SecurityUserRepository())
            {
                return Ok(secUserRepo.GetStatus(userId));
            }
        }

        [Authorize(Roles = "User")]
        [Route("PromoCodeStatus")]
        [HttpGet]
        public IHttpActionResult PromoCodeStatus(string promoName)
        {
            //int userId = GetUserId();
            //if (userId == 0)
            //{
            //    reqResponse.ErrorMessages.Add("noUser", "Invalid Data");
            //    return Ok(reqResponse);
            //}
            using (PromoCodeRepository promoRepo = new PromoCodeRepository())
            {
                DomainModel.PromoCode promoCode = promoRepo.GetByName(promoName);
                if (promoCode == null)
                {
                    reqResponse.ErrorMessages.Add("invalid", "Invalid Promo Code!");
                }
                else
                {
                    if (promoCode.IsDeleted == true)
                    {
                        reqResponse.ErrorMessages.Add("invalid", "Invalid Promo Code!");
                    }
                    else
                    {
                        reqResponse.ResponseIdStr = promoCode == null ? "" : promoCode.Id.ToString();
                    }
                }
                
               
                return Ok(reqResponse);
            }
        }

        private string UnescapeCodes(string src)
        {
            //src = src.Substring(1, src.Length - 1);
            src = src.Replace("%2C", ",");

            return src;
        }

        [Route("VehiclStatus1")]
        [HttpPost]
        public HttpResponseMessage VehiclStatus1()
        {
            var requestParams = Request.Content.ReadAsStringAsync().Result;



            bool inRide = false;
            requestParams = UnescapeCodes(requestParams);

            List<string> splitedParam = requestParams.Trim().Split(',').ToList();


            if (splitedParam.Count > 0)
            {
                using (VehicleStatusRepository vsRepo = new VehicleStatusRepository())
                {
                    inRide = vsRepo.UpdateCordinates(splitedParam[0], decimal.Parse(splitedParam[1]), splitedParam[4], splitedParam[3]);

                    if (inRide)
                    {
                        int? currentTripId = 0;
                        using (TripRepository tRepo = new TripRepository())
                        {
                            currentTripId = tRepo.GetLastVehicalOpenTrip(splitedParam[0]);
                        }
                        if (currentTripId != null)
                        {
                            using (TripCoordinatesRepository tripCorrRepo = new TripCoordinatesRepository())
                            {
                                tripCorrRepo.AddUpdate(new DomainModel.TripCoordinates()
                                {
                                    DateTime = DateTime.Now,
                                    TripId = currentTripId.Value,
                                    LatV = splitedParam[3],
                                    LongV = splitedParam[4],
                                });
                            }
                        }
                    }
                }
            }

            //var vs = new TempRepository().AddUpdate(new DomainModel.TempStatus()
            //{
            //    CreatedDate = DateTime.Now,
            //    DataStr =" sss   >>> " + sss
            //});



            string result = inRide ? "on" : "off";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return resp;

            //return "on";
        }

        [Authorize(Roles = "User")]
        [Route("GetByArea")]
        [HttpPost]
        public IHttpActionResult GetByArea(AreaModel areaModel)
        {

            using (VehicleRepository vehiclesRepo = new VehicleRepository())
            {
                return Ok(vehiclesRepo.GetAvilable());
            }
            // return Ok(new { IsDone = true, Messages = areaModel.farLeft.lat });
        }


        [Route("GetById")]
        [HttpGet]
        public IHttpActionResult GetById(string vId)
        {
            using (VehicleStatusRepository vehiclesRepo = new VehicleStatusRepository())
            {
                RequestResponse returnData = vehiclesRepo.GetByIdService(int.Parse(vId));
                return Ok(returnData);
            }
            // return Ok(new { IsDone = true, Messages = areaModel.farLeft.lat });
        }

        [Route("GetTripById")]
        [HttpGet]
        public IHttpActionResult GetTripById(string tripId)
        {
            using (TripRepository vehiclesRepo = new TripRepository())
            {
                RequestResponse returnData = vehiclesRepo.GetTripStatus(int.Parse(tripId));
                return Ok(returnData);
            }
            // return Ok(new { IsDone = true, Messages = areaModel.farLeft.lat });
        }

        [Route("RateTripById")]
        [HttpGet]
        public IHttpActionResult RateTripById(string tripId, int rate, bool isRepair)
        {
            using (TripRepository vehiclesRepo = new TripRepository())
            {
                RequestResponse returnData = vehiclesRepo.UpdateTripRate(int.Parse(tripId), rate, isRepair);
                return Ok(returnData);
            }
            // return Ok(new { IsDone = true, Messages = areaModel.farLeft.lat });
        }

        [Authorize(Roles = "User")]
        [Route("Reservation")]
        [HttpPost]
        public IHttpActionResult Reservation(VehicaleReservationModel data)
        {
            int userId = GetUserId();
            if (userId == 0)
            {
                reqResponse.ErrorMessages.Add("noUser", "Invalid Data");
                return Ok(reqResponse);
            }

            data.riderId = userId;

            using (TripRepository tripRepo = new TripRepository())
            {
                return Ok(tripRepo.TripRegister(data));
            }
        }

        [Authorize(Roles = "User")]
        [Route("GetBalance")]
        [HttpPost]
        public IHttpActionResult GetBalance()
        {
            int userId = GetUserId();
            if (userId == 0)
            {
                reqResponse.ErrorMessages.Add("noUser", "Invalid Data");
                return Ok(reqResponse);
            }

            using (TripRepository tripRepo = new TripRepository())
            {
                return Ok(tripRepo.GetBalance(userId));
            }
        }

        [Route("TripPaymentId")]
        [HttpPost]
        public IHttpActionResult TripPaymentId(string tripId, string orderId)
        {
            using (TripRepository tripRepo = new TripRepository())
            {
                return Ok(tripRepo.TripPaymentId(int.Parse(tripId), int.Parse(orderId)));
            }
        }


        [Authorize(Roles = "User")]
        [Route("Subscription")]
        [HttpPost]
        public IHttpActionResult Subscription(SubscriptionModel data)
        {
            int userId = GetUserId();
            if (userId == 0)
            {
                reqResponse.ErrorMessages.Add("noUser", "Invalid Data");
                return Ok(reqResponse);
            }

            data.riderId = userId;

            if (string.IsNullOrEmpty(data.Name) || string.IsNullOrEmpty(data.PhoneNumber.ToString()) || data.PhoneNumber <= 0 || string.IsNullOrEmpty(data.Location) || string.IsNullOrEmpty(data.DateTimeStr))
            {
                reqResponse.ErrorMessages.Add("invalidD", "Invalid Data");

                return Ok(reqResponse);
            }

            using (SubscriptionRepository subscriptionRepo = new SubscriptionRepository())
            {
                return Ok(
                    subscriptionRepo.AddUpdate(new DomainModel.SubscriptionV()
                    {
                        Name = data.Name,
                        PhoneNumber = data.PhoneNumber,
                        LocationStr=data.Location,
                        Lat=data.Lat,
                        Lng=data.Lng,
                        PickDateTime=DateTime.Parse(data.DateTimeStr),
                        DaysCount=data.DaysCount,
                        PromoCodeId=data.PromoDodeId,
                        UserId=data.riderId
                    })
                );
            }
        }

        // [Authorize(Roles = "User")]
        [Route("UploadId")]
        [HttpPost]
        public IHttpActionResult UploadId(TestModel test)//TestModel test //string base64image //[FromBody]
        {


            var bytes = Convert.FromBase64String(test.base64image);//test.base64image// a.base64image 
            Random randomObj = new Random();
            string imagePath = Guid.NewGuid().ToString().Substring(0, (10 / 2 - 1)).ToString() + randomObj.Next(10 / 2, 10 / 2).ToString();

            using (var stream = new FileStream(HttpContext.Current.Server.MapPath("~/DataImages/") + imagePath, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }

            return Ok("Done");
        }

        [Route("UploadId1")]
        [HttpPost]
        public IHttpActionResult UploadId1([FromBody]TestModel test)//TestModel test //string base64image //[FromBody]
        {
            var bytes = Convert.FromBase64String(test.base64image);//test.base64image// a.base64image 
            Random randomObj = new Random();
            string imagePath = Guid.NewGuid().ToString().Substring(0, (10 / 2 - 1)).ToString() + randomObj.Next(10 / 2, 10 / 2).ToString();

            using (var stream = new FileStream(HttpContext.Current.Server.MapPath("~/DataImages/") + imagePath, FileMode.Create))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }

            return Ok("Done");


        }

        //  [Authorize(Roles = "User")]
        [Route("UploadFile")]
        [HttpPost]
        public IHttpActionResult UploadFile()
        {
            int userId = GetUserId();
            if (userId == 0)
            {
                reqResponse.ErrorMessages.Add("noUser", "Invalid Data");
                return Ok(reqResponse);
            }
            string fileName = RandomNumber(20) + ".jpg";
            var file = HttpContext.Current.Request.Files.Count > 0 ?
                HttpContext.Current.Request.Files[0] : null;

            if (file != null && file.ContentLength > 0)
            {
                // var fileName = Path.GetFileName(file.FileName);

                var path = Path.Combine(
                    HttpContext.Current.Server.MapPath("~/DataImages"),
                    fileName
                );

                file.SaveAs(path);
                using (SecurityUserRepository secUserRepo = new SecurityUserRepository())
                {
                    var curentModel = secUserRepo.GetById(userId);
                    curentModel.IdString = fileName;
                    secUserRepo.AddUpdate(curentModel);
                }
            }

            return Ok(reqResponse);
            //return file != null ? "/uploads/" + file.FileName : null;
        }


        private int GetUserId()
        {
            var claims = (ClaimsIdentity)ClaimsPrincipal.Current.Identity;

            if (claims == null)
            {
                return 0;
            }

            var targetClaim = claims.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (targetClaim == null)
            {
                return 0;
            }

            return int.Parse(targetClaim.Value);
        }

        private string RandomNumber(int length)
        {
            Random randomObj = new Random();
            return Guid.NewGuid().ToString().Substring(0, (length / 2 - 1)).ToString() + randomObj.Next(length / 2, length / 2).ToString();
        }

        [Route("Upload")]
        [HttpPost]
        public IHttpActionResult Upload(Stream stream)
        {
            HttpMultipartParser parser = new HttpMultipartParser(stream, "file");

            if (parser.Success)
            {
                //string user = HttpUtility.UrlDecode(parser.Parameters["user"]);
                //string title = HttpUtility.UrlDecode(parser.Parameters["title"]);

                // Save the file somewhere
                File.WriteAllBytes(HttpContext.Current.Server.MapPath("~/DataImages") + "/ss.jpg", parser.FileContents);
            }
            return Ok("Done");
        }


        [Route("UploadId3")]
        [HttpPost]
        public IHttpActionResult UploadId3()//TestModel test //string base64image //[FromBody]
        {
            var request = HttpContext.Current.Request;

            if (Request.Content.IsMimeMultipartContent())
            {
                if (request.Files.Count > 0)
                {
                    var postedFile = request.Files.Get("file");
                    var title = request.Params["title"];
                    string root = HttpContext.Current.Server.MapPath("~/DataImages");
                    root = root + "/" + postedFile.FileName;
                    postedFile.SaveAs(root);
                }
            }
            return Ok("Done");

            //string base64image = "";
            //var bytes = Convert.FromBase64String(base64image);// a.base64image 

            //var request = HttpContext.Current.Request;

            //if (Request.Content.IsMimeMultipartContent())
            //{
            //    if (request.Files.Count > 0)
            //    {
            //        var postedFile = request.Files.Get("file");
            //        var title = request.Params["title"];
            //    }
            //}

            ////or full path to file in temp location
            ////var filePath = Path.GetTempFileName();

            //// full path to file in current project location
            //string filedir = Path.Combine(Directory.GetCurrentDirectory(), "NewFolder");
            //if (!Directory.Exists(filedir))
            //{ //check if the folder exists;
            //    Directory.CreateDirectory(filedir);
            //}
            //string file = Path.Combine(filedir, "a.jpg");

            ////Debug.WriteLine(File.Exists(file));


            //if (bytes.Length > 0)
            //{
            //    using (var stream = new FileStream(file, FileMode.Create))
            //    {
            //        stream.Write(bytes, 0, bytes.Length);
            //        stream.Flush();
            //    }
            //}

            //return Ok("Done");
        }

    }

    public class TestModel
    {
        public string base64image { get; set; }
    }
}
