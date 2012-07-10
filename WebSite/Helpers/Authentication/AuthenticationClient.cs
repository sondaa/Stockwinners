using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using DotNetOpenAuth.OAuth2;
using WebSite.Models;

namespace WebSite.Helpers.Authentication
{
    abstract class AuthenticationClient : WebServerClient
    {
        /// <summary>
        /// Set of scopes requested from the authorization server.
        /// </summary>
        private string[] _requestScopes;

        private string _requestUri;

        private IdentityProvider _identityProvider;

        /// <summary>
        /// We enforce the provding of a <see cref="ClientCredentialApplicator"/> such that the consumer is forced to choose the method using which
        /// the client secret is sent to the authorization server.
        /// </summary>
        /// <param name="requestScopes">Set of scopes to inquire from the authorization server.</param>
        /// <param name="dataRequestUri">Location at which to inquire the data once the request is authenticated.</param>
        public AuthenticationClient(AuthorizationServerDescription description, string clientIdentifier, 
            ClientCredentialApplicator credentialApplicator, string[] requestScopes, string dataRequestUri,
            IdentityProvider identityProvider)
            : base(description, clientIdentifier, credentialApplicator)
        {
            _requestScopes = requestScopes;
            _requestUri = dataRequestUri;
            _identityProvider = identityProvider;
        }

        /// <summary>
        /// Returns the identity of the current user from a third-party authentication provider
        /// </summary>
        /// <returns></returns>
        public LoggedInUserIdentity GetUserIdentity(string returnUrl)
        {
            IAuthorizationState authorizationState = this.ProcessUserAuthorization();

            if (authorizationState == null)
            {
                // Send the authorization request to the corresponding authorization server. If all of the providers could agree on the same format, we could
                // be doing the same code irrespective of which identity provider is being used which would be the ideal design.
                if (_identityProvider == IdentityProvider.Google)
                {
                    // TODO: Google is lame and does not support addition of extra data at the end of the call back URL. We need to use an extra
                    // state parameter for which DotNetOpenAuth does not have support yet.
                    this.RequestUserAuthorization(_requestScopes, new Uri("http://localhost:38187/authentication/googlecallback"));
                }
                else if (_identityProvider == IdentityProvider.Facebook)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                       this.RequestUserAuthorization(_requestScopes, new Uri("http://localhost:38187/authentication/facebookcallback?returnUrl=" + returnUrl));
                    }
                    else
                    {
                        this.RequestUserAuthorization(_requestScopes, new Uri("http://localhost:38187/authentication/facebookcallback"));
                    }
                }
            }
            else
            {
                // Check to see if the access token is valid and if not, try to refresh it using the refresh token
                if (authorizationState.AccessTokenExpirationUtc < DateTime.UtcNow && authorizationState.RefreshToken != null)
                {
                    this.RefreshAuthorization(authorizationState);
                }

                // Append the access token to the request URI to notify the server we are authenticated
                var request = HttpWebRequest.Create(_requestUri + "?access_token=" + authorizationState.AccessToken) as HttpWebRequest;

                // Parse the response and return an identity object
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        return this.HandleResponseStream(responseStream);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Handles the response obtained from the authorization server after user data has been successfully requested from it and converts it
        /// to a <see cref="LoggedInUserIdentity"/>.
        /// </summary>
        protected abstract LoggedInUserIdentity HandleResponseStream(System.IO.Stream responseStream);
    }
}