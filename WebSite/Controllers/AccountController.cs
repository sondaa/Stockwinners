using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.RelyingParty;
using WebSite.Models;
using DotNetOpenAuth.Messaging;
using WebSite.Helpers.Authentication;
using WebSite.Database;

namespace WebSite.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            ViewBag.InvalidEmail = false;
            ViewBag.SuccessfulChange = false;

            return this.View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(string emailAddress)
        {
            ViewBag.InvalidEmail = false;
            ViewBag.SuccessfulChange = false;
            WebSite.Infrastructure.MembershipProvider membershipProvider = Membership.Provider as WebSite.Infrastructure.MembershipProvider;
            string resetPassword = membershipProvider.ResetPassword(emailAddress, string.Empty);

            if (resetPassword == null)
            {
                ViewBag.InvalidEmail = true;
            }
            else
            {
                // Email the new password to the user
                DatabaseContext.GetInstance().StockwinnersMembers.First(swMember => swMember.EmailAddress == emailAddress).SendPasswordResetEmail(resetPassword);

                ViewBag.SuccessfulChange = true;
            }

            return this.View();
        }


        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            WebSite.Models.User user = Authentication.GetCurrentUserEagerlyLoaded();

            if (user != null)
            {
                if (user.SubscriptionId.HasValue)
                {
                    // The user has a subscription, are they routed to the login page because they have a suspended subscription?
                    ViewBag.HasSuspendedSubscription = user.Subscription.IsSuspended;
                }
                else
                {
                    // The user does not have a subscription, are they routed to this page because they have an expired trial membership?
                    ViewBag.TrialExpired = !user.IsTrialValid();
                }
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (!Url.IsLocalUrl(returnUrl))
                {
                    returnUrl = null;
                }

                if (!Authentication.AuthenticateOrRedirectStockwinnersMember(model.Email, model.Password, returnUrl, model.RememberMe))
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            Authentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                WebSite.Infrastructure.MembershipProvider membershipProvider = Membership.Provider as WebSite.Infrastructure.MembershipProvider;

                int memberId = membershipProvider.CreateStockwinnersMember(model.Email, model.Password, model.FirstName, model.LastName, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    Authentication.SetCurrentUser(new LoggedInUserIdentity()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        EmailAddress = model.Email,
                        IdentityProvider = IdentityProvider.Stockwinners,
                        IdentityProviderIssuedId = memberId.ToString()
                    });

                    return this.RedirectToAction("RegistrationSuccess");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult RegistrationSuccess()
        {
            return this.View();
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            // Only stockwinners members can change their password using our website.
            this.EnsureStockwinnersMember();

            return View();
        }

        public ActionResult InvalidProvider()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(PasswordChangeModel model)
        {
            this.EnsureStockwinnersMember();

            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(Authentication.GetCurrentUserIdentity().EmailAddress, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }

        private void EnsureStockwinnersMember()
        {
            LoggedInUserIdentity currentUser = Authentication.GetCurrentUserIdentity();

            if (currentUser.IdentityProvider != IdentityProvider.Stockwinners)
            {
                RedirectToAction("InvalidProvider");
            }
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
