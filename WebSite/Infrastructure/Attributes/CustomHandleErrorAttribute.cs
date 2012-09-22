using Stockwinners.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Infrastructure.Attributes
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            // Email the admins 
            IEmailFactory emailFactory = System.Web.Mvc.DependencyResolver.Current.GetService<IEmailFactory>();
            string subject = "Error in website";
            StringBuilder body = new StringBuilder();

            body.Append("Controller: ");
            body.Append(filterContext.Controller.ToString());
            body.Append("<br/>Exception Message: ");
            body.Append(filterContext.Exception.Message);
            body.Append("<br/>Call Stack:<br/>");
            body.Append(filterContext.Exception.StackTrace);
            body.Append("<br/>Inner Exception Message:<br/>");
            body.Append(filterContext.Exception.InnerException != null ? filterContext.Exception.InnerException.Message : "Nothing");
            body.Append("<br/>Raw URL: ");
            body.Append(filterContext.RequestContext.HttpContext.Request.RawUrl);
            
            emailFactory.CreateEmailForAdministrators(body.ToString(), subject).Send();

            // Do whatever the base class does
            base.OnException(filterContext);
        }
    }
}