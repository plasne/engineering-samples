using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Identity.Client;

namespace common
{

    public class AuthChooser
    {

        public static string TenantId
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("TENANT_ID");
            }
        }

        public static string ClientId
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("CLIENT_ID");
            }
        }

        public static string ClientSecret
        {
            get
            {
                return System.Environment.GetEnvironmentVariable("CLIENT_SECRET");
            }
        }

        public static string TenantIdConfig
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("TENANT_ID_CONFIG");
                if (string.IsNullOrEmpty(s)) return TenantId;
                return s;
            }
        }

        public static string ClientIdConfig
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("CLIENT_ID_CONFIG");
                if (string.IsNullOrEmpty(s)) return ClientId;
                return s;
            }
        }

        public static string ClientSecretConfig
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("CLIENT_SECRET_CONFIG");
                if (string.IsNullOrEmpty(s)) return ClientSecret;
                return s;
            }
        }

        public static string TenantIdVault
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("TENANT_ID_VAULT");
                if (string.IsNullOrEmpty(s)) return TenantId;
                return s;
            }
        }

        public static string ClientIdVault
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("CLIENT_ID_VAULT");
                if (string.IsNullOrEmpty(s)) return ClientId;
                return s;
            }
        }

        public static string ClientSecretVault
        {
            get
            {
                string s = System.Environment.GetEnvironmentVariable("CLIENT_SECRET_VAULT");
                if (string.IsNullOrEmpty(s)) return ClientSecret;
                return s;
            }
        }

        public static string AuthType(string key = "AUTH_TYPE")
        {
            string type = System.Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(type) && key != "AUTH_TYPE") type = System.Environment.GetEnvironmentVariable("AUTH_TYPE");
            if (string.IsNullOrEmpty(type)) return "mi";
            string[] app = new string[] { "app", "application", "service", "service_principal", "service-principal", "service principal" };
            return (app.Contains(type.ToLower())) ? "app" : "mi";
        }

        public static async Task<string> GetAccessTokenByApplication(string resourceId, string tenantId, string clientId, string clientSecret)
        {

            // builder
            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithClientSecret(clientSecret)
                .Build();

            // ensure resourceId does not have trailing /
            if (resourceId.Last() == '/') resourceId.Substring(0, resourceId.Length - 1);

            // get an access token
            string[] scopes = new string[] { $"offline_access {resourceId}/.default" };
            var acquire = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return acquire.AccessToken;

        }

        public static async Task<string> GetAccessTokenByManagedIdentity(string resourceId)
        {
            var tokenProvider = new AzureServiceTokenProvider();
            return await tokenProvider.GetAccessTokenAsync(resourceId);
        }

        public static async Task<string> GetAccessToken(string resourceId, string authTypeKey)
        {
            switch (AuthType(authTypeKey))
            {
                case "app":
                    switch (authTypeKey)
                    {
                        case "AUTH_TYPE_CONFIG":
                            return await GetAccessTokenByApplication(resourceId, TenantIdConfig, ClientIdConfig, ClientSecretConfig);
                        case "AUTH_TYPE_VAULT":
                            return await GetAccessTokenByApplication(resourceId, TenantIdVault, ClientIdVault, ClientSecretVault);
                        default:
                            throw new Exception("GetAccessToken requires an authTypeKey when using AUTH_TYPE=app");
                    }
                default:
                    return await GetAccessTokenByManagedIdentity(resourceId);
            }
        }

    }

}