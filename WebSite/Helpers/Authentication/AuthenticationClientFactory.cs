using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Models;

namespace WebSite.Helpers.Authentication
{
    class AuthenticationClientFactory
    {
        private GoogleClient _googleClient;
        private GoogleClient GoogleClient
        {
            get
            {
                if (_googleClient == null)
                {
                    _googleClient = new GoogleClient();
                }

                return _googleClient;
            }
        }

        private FacebookClient _facebookClient;
        private FacebookClient FacebookClient
        {
            get
            {
                if (_facebookClient == null)
                {
                    _facebookClient = new FacebookClient();
                }

                return _facebookClient;
            }
        }

        private static AuthenticationClientFactory _instance;
        public static AuthenticationClientFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AuthenticationClientFactory();
                }

                return _instance;
            }
        }

        public AuthenticationClient GetAuthenticationClient(IdentityProvider identityProvider)
        {
            switch (identityProvider)
            {
                case IdentityProvider.Google:
                    return this.GoogleClient;
                case IdentityProvider.Facebook:
                    return this.FacebookClient;
            }

            throw new InvalidOperationException();
        }
    }
}