﻿using Fly.BLL;
using Fly.DomainModel;
using Fly.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fly.Controllers
{
    public class AreaCController : AdminBaseController
    {
        public ActionResult Index(int? id)
        {
            ViewBag.ActiveMenu = 1;

            AreasT areaModel = new AreasT();

            if (id != null)
            {
                using (AreasTRepository vehiclesProxy = new AreasTRepository())
                {
                    areaModel = vehiclesProxy.GetById((int)id);
                }

                areaModel.CountriesList = new CountriesRepository().GetAll().ToList();
                return PartialView("_AddArea", areaModel);
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    areaModel.CountriesList = new CountriesRepository().GetAll().ToList();
                    return PartialView("_AddArea", areaModel);
                }
                else
                {
                    return View(areaModel);
                }
            }
        }

        public ActionResult loadData(String searchtoken)
        {

            AreasTRepository areaProxy = new AreasTRepository();

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
                    Name= r.Name,
                    CName=r.Countries.Name
                }).AsQueryable()

            };

            var test = Json(
                toSerialize,
                JsonRequestBehavior.AllowGet);
            return test;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(AreasT model)
        {
            if (model.Id == 0)
            {
                ModelState["Id"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                using (AreasTRepository areaProxy = new AreasTRepository())
                {
                    reqResponse = areaProxy.AddUpdate(model);

                    return Json(reqResponse, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                reqResponse.ErrorMessages.Add("invalidData", OperationLP.InvalidData);
                return Json(reqResponse, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Delete(int id)
        {
            using (AreasTRepository areaProxy = new AreasTRepository())
            {
                bool isSuccess = areaProxy.Delete(id);
                return Json(new { isSuccess = isSuccess, responseType = isSuccess ? 1 : 2, msg = isSuccess ? OperationLP.DeleteSuccessfully : OperationLP.DeleteFailuer }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}