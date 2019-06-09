using Fly.DomainModel.Helper;
using Fly.WebUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Fly.Controllers
{
    [SecurityAttribute]
    [HandleError]
    [Authorize]
    [ValidateInput(false)]
    public class AdminBaseController : Controller
    {
        // public SecurityMembershipProvider membershipProvider;
        public log4net.ILog logger;
        //private List<UserTenant> tenants;
        protected RequestResponse reqResponse;
        #region Handle constructor
        public AdminBaseController()
        {
            reqResponse = new RequestResponse();
            //Init General variables
            // membershipProvider = new SecurityMembershipProvider();
            logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }
        #endregion

        #region Handle Execute event
        //protected override void ExecuteCore()
        //{
        //    //Init Culture         
        //    string CultureName = "en-GB";

        //    Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(CultureName);
        //    Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
        //    //Execute action
        //    base.ExecuteCore();
        //}
        //protected override bool DisableAsyncSupport
        //{
        //    get { return true; }
        //}

        #endregion

        #region Handle Security Attribute
        [AttributeUsage(AttributeTargets.All)]
        public class SecurityAttribute : ActionFilterAttribute
        {
            public string ActionMap { get; set; }
            public bool NotSecured { get; set; }

            //  public log4net.ILog logger;
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                if (WebUiUtility.CurrentUser != null)
                {
                    if (WebUiUtility.CurrentUser.Id > 0)
                    {
                        // logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                        if (!NotSecured)
                        {
                            string featureName = (filterContext.ActionDescriptor.ControllerDescriptor).ControllerName;
                            string actionName = (filterContext.ActionDescriptor).ActionName;

                            filterContext.Controller.ViewData["RightMenuSelectedItem"] = featureName;
                            filterContext.Controller.ViewData["RightMenuSelectedItemAction"] = actionName;

                            if (ActionMap != null) actionName = ActionMap;

                            //if (WebUiUtility.CurrentUser.Id == 0)
                            //{
                            //    filterContext.Result = new ViewResult { ViewName = "~/Views/Login/Index.cshtml" };
                            //}
                            //using (UserRepository securityUserProxy = new UserRepository())
                            //{
                            //    if (featureName != "Home" && !securityUserProxy.IsUserAlowed(WebUiUtility.CurrentUser.Id, featureName, actionName))
                            //    {

                            //        ViewDataDictionary error = new ViewDataDictionary();
                            //        error.Add(new KeyValuePair<string, object>("error", ".AccessDenied"));

                            //        //logger.Error("[control]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["controller"]) + ",[action]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["action"]) + ",[errormessage]:" + AccountLP.AccessDenied);

                            //        filterContext.Result = new ViewResult { ViewName = "~/Views/Shared/Error.cshtml", ViewData = error };

                            //        //}
                            //    }
                            //}

                        }
                    }
                    else
                    {
                        System.Web.Security.FormsAuthentication.SignOut();

                    }
                }


                base.OnActionExecuting(filterContext);
            }
        }

        //private SecurityUser _currentUser;
        //public SecurityUser currentUser
        //{
        //    get
        //    {
        //        return _currentUser = membershipProvider.GetSecurityUser(User.Identity.Name, true /* userIsOnline */);
        //    }
        //    set
        //    {
        //        _currentUser = value;
        //    }
        //}

        #endregion

        #region Handle Exceptions
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

                //if (filterContext.HttpContext.Request.IsAjaxRequest() && filterContext.Exception != null)
                //{
                //    logger.Error("[IsAjaxRequest]:yes,[control]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["controller"]) + ",[action]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["action"]) + ",[errormessage]:" + e.Message + "::" + e.StackTrace);
                //    filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                //    filterContext.Result = Json(new { success = false, responseText = e.Message }, JsonRequestBehavior.AllowGet);
                //    filterContext.ExceptionHandled = true;
                //}
                //else
                //{
                logger.Error(e.StackTrace + " [IsAjaxRequest]:no,[control]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["controller"]) + ",[action]:" + Convert.ToString(filterContext.Controller.ControllerContext.RouteData.Values["action"]) + ",[errormessage]:" + e.Message + " - " + e.InnerException.InnerException.Message);
                // TempData["message"] = e.Message;
                filterContext.Result = this.Redirect(Request.UrlReferrer.ToString());
                //}

                logger.Info("Handle Exception end");
            }
            catch (Exception exp)
            {
                logger.Error("Exception in exception : " + exp.Message);
            }
        }
        #endregion
    }
}