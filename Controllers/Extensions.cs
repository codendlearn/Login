using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Controllers
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether the client is configured to use PKCE.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="client_id">The client identifier.</param>
        /// <returns></returns>
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string client_id)
        {
            if (!string.IsNullOrWhiteSpace(client_id))
            {
                var client = await store.FindEnabledClientByIdAsync(client_id);
                return client?.RequirePkce == true;
            }

            return false;
        }

        public static void AddCertificateFromFile(this IIdentityServerBuilder builder, IConfigurationSection options, ILogger logger)
        {
            var path = "";
            var keyFilePath = options.GetValue<string>(path);
            var keyFilePassword = options.GetValue<string>("ApnePassword");

            if (File.Exists(keyFilePath))
            {
                logger.LogDebug($"SigninCredentialExtension adding key from file {keyFilePath}");

                // You can simply add this line in the Startup.cs if you don't want an extension. 
                // This is neater though ;)
                builder.AddSigningCredential(new X509Certificate2(keyFilePath, keyFilePassword));
            }
            else
            {
                logger.LogError($"SigninCredentialExtension cannot find key file {keyFilePath}");
            }
        }
    }
}
