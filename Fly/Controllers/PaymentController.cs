using Fly.BLL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Fly.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index(bool success, int order)
        {
            string ff = "";
            foreach (string key in Request.Form.AllKeys)
            {
                ff += Request.Form[key];
            }

            using (TempRepository bb = new TempRepository())
            {
                bb.AddUpdate(new DomainModel.TempStatus()
                {
                    DataStr = ff,
                    CreatedDate = DateTime.Now
                });
            }

            var bodyStream = new StreamReader(Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();


            using (TempRepository bb=new TempRepository())
            {
                bb.AddUpdate(new DomainModel.TempStatus()
                {
                    DataStr=bodyText,
                    CreatedDate=DateTime.Now
                });
            }
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