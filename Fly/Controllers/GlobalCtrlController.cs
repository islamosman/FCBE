using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fly.Controllers
{
    public class GlobalCtrlController : AdminBaseController
    {
        // GET: Administration/GlobalCtrl
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TableStructure(int colmnCount, int operationsColumnCount)
        {
            ViewBag.columnCount = colmnCount;
            ViewBag.columnOperationCount = operationsColumnCount;

            return PartialView("_TableStructure");
        }
    }
}