using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fly.BLL;
using Fly.DAL;

namespace Fly.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ScotterRepo scoterRepo = new ScotterRepo();

            return View(scoterRepo.GetAll());
        }

        [HttpGet]
        public ActionResult AddNew(int? scotterId)
        {
            Scotter scoModel = new Scotter();

            return View(scoModel);
        }

        [HttpPost]
        public ActionResult AddNew(Scotter model)
        {
            ScotterRepo scoterRepo = new ScotterRepo();
            scoterRepo.Add(model);

            return RedirectToAction("AddNew", new { scotterId = model.Id });
        }
    }
}
