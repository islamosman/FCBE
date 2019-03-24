using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Fly.BLL;
using Fly.DomainModel.Helper;

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

        [Route("VehiclStatus1")]
        [HttpPost]
        public HttpResponseMessage VehiclStatus1()
        {
            var sss = Request.Content.ReadAsStringAsync().Result;
            
            var vs = new TempRepository().AddUpdate(new DomainModel.TempStatus()
            {
                CreatedDate = DateTime.Now,
                DataStr =" sss   >>> " + sss
            });
            string result = "on";
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return resp;

            //return "on";
        }

        [Route("GetByArea")]
        [HttpPost]
        public IHttpActionResult GetByArea(AreaModel areaModel)
        {
            using (VehicleRepository vehiclesRepo=new VehicleRepository())
            {
                return Ok(vehiclesRepo.GetAvilable());
            }
           // return Ok(new { IsDone = true, Messages = areaModel.farLeft.lat });
        }


        [Route("Reservation")]
        [HttpPost]
        public IHttpActionResult Reservation(VehicaleReservationModel data)
        {
            using (TripRepository tripRepo=new TripRepository())
            {
                return Ok(tripRepo.TripRegister(data));
            }
        }
    }
}
