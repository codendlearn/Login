// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api", "Acme Fireworks Co. payroll")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {
                new Client
                {
                    ClientId = "spa",
                    ClientName = "Single Page Javascript App",
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    RequireClientSecret = false,
                    RequireConsent = false,
                    RedirectUris = { "http://localhost:4200/callback.html",
                                       "http://localhost:4200/signing-redirect"},
                    AllowedCorsOrigins = { "http://localhost:4200" },
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "api"
                    }
                }
            };
        }

        internal static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser { SubjectId = "1", Username = "alice", Password = "alice",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.Email, "AliceSmith@email.com")
                    }
                },
                new TestUser { SubjectId = "11", Username = "bob", Password = "bob",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.Email, "BobSmith@email.com")
                    }
                }
            };
        }
    }
}
