using Fly.BLL;
using Fly.DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fly.Controllers
{
    public class ClientsController : AdminBaseController
    {
        public ActionResult Index()
        {
            ViewBag.ActiveMenu = 1;

            return View();
        }

        public ActionResult loadData(String searchtoken)
        {

            SecurityUserRepository areaProxy = new SecurityUserRepository();

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
                    Name = r.FullName,
                    Idstr=r.IdString
                }).AsQueryable()

            };

            var test = Json(
                toSerialize,
                JsonRequestBehavior.AllowGet);
            return test;
        }

        [HttpPost]
        public ActionResult GetTrips(int clientId)
        {
            using (TripRepository tripRepo = new TripRepository())
            {
                return View(tripRepo.GetAllByUser(clientId).ReturnedObject);
            }

        }
    }
}