using Fly.BLL;
using Fly.DomainModel;
using Fly.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fly.Controllers
{
    public class SubscriptionsController : AdminBaseController
    {
        public ActionResult Index(int? id)
        {
            ViewBag.ActiveMenu = 1;

            SubscriptionV areaModel = new SubscriptionV();

            if (id != null)
            {
                using (SubscriptionRepository vehiclesProxy = new SubscriptionRepository())
                {
                    areaModel = vehiclesProxy.GetById((int)id);
                }

                return PartialView("_AddSubscription", areaModel);
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    return PartialView("_AddSubscription", areaModel);
                }
                else
                {
                    return View(areaModel);
                }
            }
        }

        public ActionResult loadData(String searchtoken)
        {

            SubscriptionRepository areaProxy = new SubscriptionRepository();

            // get start (paging start index) and length (page size for pagging)
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            // get sort columns value
            var sortColumn =
                Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() +
                                       "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            var v = areaProxy.get(skip, pageSize, searchtoken, sortColumn, sortColumnDir)
                  .Distinct().ToList();//.OrderBy(sortColumn + " " + sortColumnDir)

            int totalRecords = areaProxy.Getcount(searchtoken);
            var toSerialize = new
            {
                draw = draw,
                recordsFiltered = totalRecords,
                recordsTotal = totalRecords,

                data = v.Select(r => new
                {
                    r.Id,
                    Name = r.Name,
                   r.DaysCount,
                   r.LocationStr,
                   r.PhoneNumber,
                    PickDateTime= r.PickDateTime.ToString("dd/MM/yyyy"),
                   r.IsDone
                }).AsQueryable()

            };

            var test = Json(
                toSerialize,
                JsonRequestBehavior.AllowGet);
            return test;
        }

        [HttpPost]
        public ActionResult ChangeDone(int uId, bool activeV)
        {
            using (SubscriptionRepository secUserRepo = new SubscriptionRepository())
            {
                secUserRepo.DoneById(uId, activeV);
            }

            return Json(new { success = reqResponse.IsDone, message = reqResponse.IsDone ? OperationLP.SuccessMsg : reqResponse.ErrorMessegesStr });
        }
    }
}