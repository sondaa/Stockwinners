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

namespace WebSite.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        /// <summary>
        /// Helper class used to perform OpenId authentication.
        /// </summary>
        private static OpenIdRelyingParty OpenIdAuthenticator = new OpenIdRelyingParty();

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult OpenIdAuthenticate(IdentityProvider identityProvider, string returnUrl)
        {
            IAuthenticationResponse authenticationResponse = OpenIdAuthenticator.GetResponse();

            if (authenticationResponse == null)
            {
                // The user is just starting the authentication process

                // Obtain the link of the identity provider
                string identityProviderUri = this.GetIdentityProviderUri(identityProvider);
                Identifier id;

                // Try obtaining an OpenID identifier for the provider
                if (Identifier.TryParse(identityProviderUri, out id))
                {
                    try
                    {
                        // Forward the user to the provider
                        return OpenIdAuthenticator.CreateRequest(id).RedirectingResponse.AsActionResult();
                    }
                    catch (ProtocolException protocolException)
                    {
                        ViewData["Message"] = protocolException.Message;

                        return View("Login");
                    }
                }
            }
            else
            {
                // We are getting the response from the identity provider
                switch (authenticationResponse.Status)
                {
                    case AuthenticationStatus.Authenticated:
                        // Store the authentication result
                        FormsAuthentication.SetAuthCookie(authenticationResponse.ClaimedIdentifier, false);

                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return this.Redirect(returnUrl);
                        }
                        else
                        {
                            // TODO: forward to home page
                            return this.RedirectToAction("Index", "Home");
                        }
                    case AuthenticationStatus.Canceled:
                        ViewData["Message"] = "Authentication process cancelled at provider. Please try again.";
                        return View("Login");
                    case AuthenticationStatus.Failed:
                        ViewData["Message"] = authenticationResponse.Exception.Message;
                        return View("Login");
                }
            }

            // Should not get to here ever. This is just to make the compiler happy
            return new EmptyResult();
        }

        private string GetIdentityProviderUri(IdentityProvider identityProvider)
        {
            NameValueCollection providers = ConfigurationManager.GetSection("identityProviders") as NameValueCollection;

            if (providers != null)
            {
                return providers[((int)identityProvider).ToString()];
            }

            return string.Empty;
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

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
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.Email, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.Email, createPersistentCookie: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(PasswordChangeModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
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

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
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
