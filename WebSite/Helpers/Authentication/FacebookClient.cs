using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebSite.Helpers.Authentication
{
    class FacebookClient : AuthenticationClient
    {
        static readonly AuthorizationServerDescription Description = new AuthorizationServerDescription
        {
            AuthorizationEndpoint = new Uri("https://www.facebook.com/dialog/oauth"),
            TokenEndpoint = new Uri("https://graph.facebook.com/oauth/access_token"),
            ProtocolVersion = ProtocolVersion.V20,
        };

        public FacebookClient()
            : base(Description,
            clientIdentifier: "384974754900125",
            credentialApplicator: ClientCredentialApplicator.PostParameter("731e7e962fd4edcc67589ea87c7f6b7d"),
            requestScopes:new string[] { }, // Not requesting anything beyond the default from Facebook
            dataRequestUri: "https://graph.facebook.com/me",
            identityProvider: Models.IdentityProvider.Facebook)
        {
        }

        protected override Models.UserIdentity HandleResponseStream(System.IO.Stream responseStream)
        {
            using (StreamReader reader = new StreamReader(responseStream))
            {
                JObject parsedJson = JsonConvert.DeserializeObject(reader.ReadToEnd()) as JObject;

                return new Models.UserIdentity()
                {
                    EmailAddress = (string)parsedJson["email"],
                    FirstName = (string)parsedJson["first_name"],
                    LastName = (string)parsedJson["last_name"],
                    Id = (string)parsedJson["id"],
                    IdentityProvider = Models.IdentityProvider.Facebook
                };
            }
        }
    }
}