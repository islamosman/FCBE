using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Fly.BLL;

namespace Fly.Controllers
{
    [RoutePrefix("api/Vehicles")]
    public class VehiclesController : BaseApiController
    {
        [Authorize(Roles = "Admin")]
        [Route("GetVehicls")]
        [HttpPost]
        public IHttpActionResult GetVehicls() {
            var vs = new VehicleRepository().GetAll().ToList();
            return Ok(new { result= vs});
        }
    }
}
