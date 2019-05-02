using Fly.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fly.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index(bool success, int order)
        {
            ViewBag.IsSuccess = false;
            if (success)
            {
                using (TripRepository tripRepo = new TripRepository())
                {
                    tripRepo.TripPaymentDone(order);
                }

                ViewBag.IsSuccess = true;
            }
            return View();
        }

        [HttpPost]
        public ActionResult PeymentMethod(bool success, int order)
        {
            ViewBag.IsSuccess = false;
            if (success)
            {
                using (TripRepository tripRepo = new TripRepository())
                {
                    tripRepo.TripPaymentDone(order);
                }

                ViewBag.IsSuccess = true;
            }
            return View();
        }

    }
}