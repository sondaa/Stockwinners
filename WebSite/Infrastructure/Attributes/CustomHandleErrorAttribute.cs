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
            body.Append("<br/>Raw URL: ");
            body.Append(filterContext.RequestContext.HttpContext.Request.RawUrl);

            for (Exception exception = filterContext.Exception; exception != null; exception = exception.InnerException)
            {
                this.PrintException(exception, body);
            }
            
            emailFactory.CreateEmailForAdministrators(body.ToString(), subject).Send();

            // Do whatever the base class does
            base.OnException(filterContext);
        }

        private void PrintException(Exception exception, StringBuilder builder)
        {
            builder.Append("<br/>Exception Message: ");
            builder.Append(exception.Message);
            builder.Append("<br/>Call Stack:<br/>");
            builder.Append(exception.StackTrace);
            builder.Append("================================");
        }
    }
}