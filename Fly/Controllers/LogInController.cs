using Fly.WebUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Fly.DomainModel;
using Fly.BLL;
using Fly.DomainModel.Helper;

namespace Fly.Controllers
{
    public class LogInController : Controller
    {
        protected virtual ActionResult ReturnOperationResult(bool successResult, string messageResult)
        {
            if (!successResult)
            {
                this.ModelState.AddModelError(string.Empty, messageResult);
            }

            if (this.Request.IsAjaxRequest())
            {
                // RedirectToAction("")
                return this.Json(new { success = successResult, message = messageResult }, JsonRequestBehavior.AllowGet);
            }

            return this.View();

        }
        #region Handle Exceptions
        //======================================================================
        //by moataz radwan 1-13-2016
        //======================================================================     
        protected override void OnException(ExceptionContext filterContext)
        {
            try
            {
                // logger.Info("Handle Exception start");

                //====================================================
                //Handle exceptions logging
                //====================================================
                Exception e = filterContext.Exception;
                filterContext.ExceptionHandled = true;
                string innerMessage = "";
                if (e.InnerException != null)
                {
                    innerMessage = e.InnerException.Message;
                }

                if (filterContext.Exception.Message == "Not auth")
                {
                    // filterContext.Result = new RedirectResult("~/Login/InvalidTocken", true);
                    filterContext.Result = this.Redirect("~/Login/InvalidTocken");
                    //filterContext.Result = this.Redirect("/InvalidTocken");
                }
                if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
                {
                    // logger.Error("[IsAjaxRequest]:yes,[control]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["controller"]) + ",[action]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["action"]) + ",[errormessage]:" + e.Message + "::" + e.StackTrace);
                    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    filterContext.Result = Json(new { success = false, responseText = e.Message }, JsonRequestBehavior.AllowGet);
                    filterContext.ExceptionHandled = true;
                }
                //else
                //{
                //    //  logger.Error("[IsAjaxRequest]:no,[control]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["controller"]) + ",[action]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["action"]) + ",[errormessage]:" + e.Message + " - " + e.InnerException.InnerException.Message);
                //    TempData["message"] = e.Message;
                //    filterContext.Result = this.Redirect(Request.UrlReferrer.ToString());
                //}

                //logger.Info("Handle Exception end");
            }
            catch (Exception exp)
            {
                //logger.Error("[control]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["controller"]) + ",[action]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["action"]) + ",[errormessage]:Error during handle exception:" + exp.Message);
            }
        }
        #endregion

        // GET: LogIn
        [AllowAnonymous]
        //[HttpGet]
        public ActionResult Index()
        {
            
            return View(new LoginViewModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult IndexPost(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                #region prepare login model
                model.Password = model.Password;
                model.UserName = model.UserName.Trim();

                using (SecurityUserRepository secProxy = new SecurityUserRepository())
                {
                    Fly.DomainModel.SecurityUser currentUser = secProxy.GetUser(model.UserName.Trim(), WebUiUtility.Encrypt(model.Password));

                    if (currentUser != null)
                    {
                        WebUiUtility.CurrentUser = currentUser;
                        string tocken = "";
                       
                        WebUiUtility.CurrentUser = currentUser;

                        FormsAuthentication.SetAuthCookie(model.UserName + ":" + tocken, model.RememberMe);
                        // RedirectToAction("index", "Home");
                        return Json(new { success = true, message = "Incorrect user name or password", forgotpassword = ViewBag.forgotpassword });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Incorrect user name or password", forgotpassword = ViewBag.forgotpassword });
                    }
                }
            }
            else
            {
                return Json(new { success = false, message = "Incorrect user name or password", forgotpassword = ViewBag.forgotpassword });
            }

            #endregion

        }

        public ActionResult SignOut()
        {
            //SignOut
            FormsAuthentication.SignOut();
            //Clear Session 
            HttpContext.Session["CurrentConfiguration"] = null;
            HttpContext.Session["CurrentUser"] = null;
            HttpContext.Session.RemoveAll();
            return RedirectToAction("Index", "LogIn");
        }

        public ActionResult agreement()
        {
            return View();
        }

        public ActionResult terms()
        {
            return View();
        }

        public ActionResult privacy()
        {
            return View();
        }
    }
}