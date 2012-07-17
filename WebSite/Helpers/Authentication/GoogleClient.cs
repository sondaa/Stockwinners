using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebSite.Models;

namespace WebSite.Helpers.Authentication
{
    class GoogleClient : AuthenticationClient
    {
        static readonly AuthorizationServerDescription Description = new AuthorizationServerDescription
        {
            AuthorizationEndpoint = new Uri("https://accounts.google.com/o/oauth2/auth"),
            TokenEndpoint = new Uri("https://accounts.google.com/o/oauth2/token"),
            ProtocolVersion = ProtocolVersion.V20,
        };

        public GoogleClient()
            : base(Description, 
            clientIdentifier: ConfigurationManager.AppSettings["GoogleClientID"], 
            // Guys at Google somehow require this to be in POST form. Documenting it in their docs would have been nice.
            credentialApplicator: ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["GoogleSecretKey"]), 
            requestScopes: new string[] { "https://www.googleapis.com/auth/userinfo.profile" }, 
            dataRequestUri: "https://www.googleapis.com/oauth2/v2/userinfo",
            identityProvider: IdentityProvider.Google)
        {
        }

        protected override LoggedInUserIdentity HandleResponseStream(Stream responseStream)
        {
            using (var streamReader = new StreamReader(responseStream))
            {
                JObject parsedJson = JsonConvert.DeserializeObject(streamReader.ReadToEnd()) as JObject;

                return new LoggedInUserIdentity()
                {
                    EmailAddress = (string)parsedJson["email"],
                    FirstName = (string)parsedJson["given_name"],
                    LastName = (string)parsedJson["family_name"],
                    IdentityProviderIssuedId = (string)parsedJson["id"],
                    IdentityProvider = IdentityProvider.Google
                };
            }
        }
    }
}