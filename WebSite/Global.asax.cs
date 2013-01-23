using Stockwinners;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WebSite.Database;
using WebSite.Helpers.Authentication;

namespace WebSite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_EndRequest()
        {
        }

        protected void FormsAuthentication_OnAuthenticate(object sender, FormsAuthenticationEventArgs args)
        {
            HttpRequest request = args.Context.Request;
            HttpResponse response = args.Context.Response;
            HttpCookie authenticationCookie = request.Cookies[FormsAuthentication.FormsCookieName];

            // Anybody logged in?
            if (authenticationCookie != null)
            {
                // Check that the user has a valid account and if so update their last login time.
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authenticationCookie.Value);

                // Does the cookie contain a valid cookie?
                if (ticket != null)
                {
                    // Are we dealing with an old cookie, the expiry of which is not the same as the timeout period in the settings?
                    // If so, update the cookie with the new settings
                    if ((ticket.Expiration - ticket.IssueDate) != FormsAuthentication.Timeout)
                    {
                        // Create new cookie with new settings
                        HttpCookie newCookie = FormsAuthentication.GetAuthCookie(ticket.Name, ticket.IsPersistent);

                        // Copy over the current user data
                        FormsAuthenticationTicket newTicket = FormsAuthentication.Decrypt(newCookie.Value);
                        newTicket = new FormsAuthenticationTicket(newTicket.Version, newTicket.Name, newTicket.IssueDate, newTicket.Expiration, newTicket.IsPersistent, ticket.UserData);
                        
                        // Update the cookie and add it to request
                        newCookie.Value = FormsAuthentication.Encrypt(newTicket);
                        response.Cookies.Add(newCookie);
                    }

                    // Check the user's state every day
                    DateTime now = DateTime.UtcNow.AddMinutes(10); // 10 minutes error buffer
                    if (ticket.Expired || (ticket.Expiration.ToUniversalTime() - now) <= (now - ticket.IssueDate.ToUniversalTime()))
                    {
                        WebSite.Models.User user = WebSite.Helpers.Authentication.Authentication.GetCurrentUser();

                        if (user != null)
                        {
                            // Update the user's last login time
                            IDatabaseContext database = System.Web.Mvc.DependencyResolver.Current.GetService<DatabaseContext>();

                            if (database != null)
                            {
                                user.LastLoginDate = DateTime.UtcNow;
                                database.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
    }
}