using Fly.BLL;
using Fly.DomainModel;
using Fly.DomainModel.Helper;
using Fly.Resources;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
//using QRBarcodeService;
using ThoughtWorks.QRCode.Codec;

namespace Fly.Controllers
{
    public class VehiclesAController : AdminBaseController
    {
        //public log4net.ILog logger;
        //public VehiclesAController()
        //{
        //    logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //}

        public ActionResult Image(string imageId)
        {
            //try
            //{
            imageId = WebUI.Helpers.WebUiUtility.Decrypt(imageId);

            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder
            {
                QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE,
                QRCodeScale = 4,
                QRCodeVersion = 8,
                QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H
            };
            Image image = qrCodeEncoder.Encode(imageId);


            using (var ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);

                image.Dispose();
                return File(ms.ToArray(), "image/jpeg");
            }
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex.Message + " > " + ex.InnerException.Message + " > " + ex.StackTrace);
            //    return null;
            //}
        }

        public ActionResult Index(int? id)
        {
            ViewBag.ActiveMenu = 1;

            Vehicles areaModel = new Vehicles();

            if (id != null)
            {
                using (VehicleRepository vehiclesProxy = new VehicleRepository())
                {
                    areaModel = vehiclesProxy.GetById((int)id);
                }

                areaModel.vCategory = new VehiclesCategoryRepository().GetAll().Select(x => new LukUpModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
                areaModel.vBrand = new VehiclesBrandRepositroy().GetAll().Select(x => new LukUpModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
                areaModel.AreasList = new AreasTRepository().GetAll().Select(x => new LukUpModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    extraValue = x.AreaCoordinates
                }).ToList();
                //areaModel.VehicleSpecs = new VehicleSpecs();

                return PartialView("_AddVehicles", areaModel);
            }
            else
            {
                if (Request.IsAjaxRequest())
                {
                    areaModel = new Vehicles()
                    {
                        vCategory = new VehiclesCategoryRepository().GetAll().Select(x => new LukUpModel()
                        {
                            Id = x.Id,
                            Name = x.Name
                        }).ToList(),
                        vBrand = new VehiclesBrandRepositroy().GetAll().Select(x => new LukUpModel()
                        {
                            Id = x.Id,
                            Name = x.Name
                        }).ToList(),
                        AreasList = new AreasTRepository().GetAll().Select(x => new LukUpModel()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            extraValue = x.AreaCoordinates
                        }).ToList(),
                        VehicleSpecs = new VehicleSpecs(),

                    };
                    return PartialView("_AddVehicles", areaModel);
                }
                else
                {
                    return View(areaModel);
                }
            }
        }

        public ActionResult loadData(String searchtoken)
        {

            VehicleRepository areaProxy = new VehicleRepository();

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
                    VName = r.Name,
                    PlateNo = r.PlateNo,
                    QRCode = r.UniqueId,
                    imageId = WebUI.Helpers.WebUiUtility.Encrypt(r.UniqueId),
                    IsActive = r.IsActive,
                    InRide = r.VehicleStatus.InRide,
                    InService = r.VehicleStatus.InService,
                    BatteryStatus = r.VehicleStatus.BatteryStatus
                }).AsQueryable()

            };

            var test = Json(
                toSerialize,
                JsonRequestBehavior.AllowGet);
            return test;
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Add(Vehicles model)
        {
            if (model.Id == 0)
            {
                ModelState["Id"].Errors.Clear();
                ModelState["VehicleSpecs.ModelId"].Errors.Clear();
                ModelState["VehicleStatus.VehicleId"].Errors.Clear();
                ModelState["VehicleStatus.BatteryStatus"].Errors.Clear();
                ModelState["VehicleStatus.InRide"].Errors.Clear();
                //ModelState["VehicleStatus.InService"].Errors.Clear();
            }

            if (ModelState.IsValid)
            {
                string fileName = ""; string fileName2 = "";
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        fileName = Path.GetFileName(fileContent.FileName);

                        fileName = Guid.NewGuid().ToString().Substring(0, 6).ToString() + fileName;
                        // Add file
                        var path = Path.Combine(Server.MapPath("~/DataImages/"), fileName);
                        fileContent.SaveAs(path);
                    }
                }
                if (!string.IsNullOrEmpty(fileName))
                {
                    model.ImageName = fileName;
                }
                if (!string.IsNullOrEmpty(fileName2))
                {
                    model.UniqueId = fileName2;
                }
                using (VehicleRepository areaProxy = new VehicleRepository())
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
            using (VehicleRepository areaProxy = new VehicleRepository())
            {
                bool isSuccess = areaProxy.Delete(id);
                return Json(new { isSuccess = isSuccess, responseType = isSuccess ? 1 : 2, msg = isSuccess ? OperationLP.DeleteSuccessfully : OperationLP.DeleteFailuer }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult GetBrandModels(int brandId)
        {
            using (VehiclesModelRepository vModelProxy = new VehiclesModelRepository())
            {
                return Json(vModelProxy.GetByBrand(brandId).Select(x => new LukUpModel()
                {
                    Id = x.Id,
                    Name = x.Name
                }));
            }

        }
    }
}