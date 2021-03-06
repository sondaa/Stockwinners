﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Helpers.Authentication;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class AuthenticationController : WebSite.Infrastructure.ControllerBase
    {
        /// <summary>
        /// Called by the Google server.
        /// </summary>
        /// <returns></returns>
        public void GoogleCallback()
        {
            Authentication.AuthenticateOrRedirect(IdentityProvider.Google, returnUrl: null);
        }

        /// <summary>
        /// Called by the Facebook server.
        /// </summary>
        /// <param name="returnUrl"></param>
        public void FacebookCallback(string returnUrl)
        {
            Authentication.AuthenticateOrRedirect(IdentityProvider.Facebook, returnUrl);
        }

        /// <summary>
        /// Authenticates via an OAuth2 authentication provider.
        /// </summary>
        /// <param name="identityProvider"></param>
        /// <returns></returns>
        public void OAuthAuthenticate(IdentityProvider identityProvider, string returnUrl)
        {
            // Only allow local URLs as return URLs
            if (returnUrl != null && !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = null;
            }

            Authentication.AuthenticateOrRedirect(identityProvider, returnUrl);
        }
    }
}
