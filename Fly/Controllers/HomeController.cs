using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.BLL;
using Fly.DomainModel;

namespace Fly.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (VehicleRepository scoterRepo = new VehicleRepository())
            {
                return View(scoterRepo.GetAll().ToList());
            }
        }

        [HttpGet]
        public ActionResult AddNew(int? scotterId)
        {
            Vehicles scoModel = new Vehicles();
            
            return View(scoModel);
        }

        [HttpPost]
        public ActionResult AddNew(Vehicles model)
        {
            using (VehicleRepository scoterRepo = new VehicleRepository())
            {
                scoterRepo.Add(model);
            }
            return RedirectToAction("AddNew", new { scotterId = model.Id });
        }
    }
}
