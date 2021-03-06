﻿using System;
using System.Collections.Generic;
using System.Configuration;
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
            clientIdentifier: ConfigurationManager.AppSettings["FacebookClientID"],
            credentialApplicator: ClientCredentialApplicator.PostParameter(ConfigurationManager.AppSettings["FacebookSecretKey"]),
            requestScopes:new string[] { "email" }, // We'd like the "email" scope from Facebook in addition to what Facebook defines as "basic"
            dataRequestUri: "https://graph.facebook.com/me",
            identityProvider: Models.IdentityProvider.Facebook)
        {
        }

        protected override Models.LoggedInUserIdentity HandleResponseStream(System.IO.Stream responseStream)
        {
            using (StreamReader reader = new StreamReader(responseStream))
            {
                JObject parsedJson = JsonConvert.DeserializeObject(reader.ReadToEnd()) as JObject;

                return new Models.LoggedInUserIdentity()
                {
                    EmailAddress = (string)parsedJson["email"],
                    FirstName = (string)parsedJson["first_name"],
                    LastName = (string)parsedJson["last_name"],
                    IdentityProviderIssuedId = (string)parsedJson["id"],
                    IdentityProvider = Models.IdentityProvider.Facebook
                };
            }
        }
    }
}