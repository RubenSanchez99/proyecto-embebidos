// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Identity.API
{
    public static class Config
    {
        // Identity resources are data like user ID, name, or email address of a user
        // see: http://docs.identityserver.io/en/release/configuration/resources.html
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("basket", "Basket Service"),
                new ApiResource("orders", "Orders Service"),
                new ApiResource("webshoppingagg", "Web Shopping Aggregator"),
                new ApiResource("orders.signalrhub", "Ordering Signalr Hub")
            };
        }

        // client want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256())
                    },
                    ClientUri = $"http://192.168.1.70:5100/",                             // public uri of the client
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AllowAccessTokensViaBrowser = false,
                    RequireConsent = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RedirectUris = new List<string>
                    {
                        $"http://192.168.1.70:5100/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        $"http://192.168.1.70:5100/signout-callback-oidc"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "basket",
                        "orders",
                        "webshoppingagg"
                    }
                },

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "js",
                    ClientName = "eShop SPA OpenId Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris =           { $"http://localhost:5106/" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"http://localhost:5106/" },
                    AllowedCorsOrigins =     { $"http://localhost:5106/" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "orders",
                        "basket",
                        "webshoppingagg"
                    }
                },
                
                new Client
                {
                    ClientId = "postman",
                    ClientName = "Postman Test Client",
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris =           { $"https://www.getpostman.com/oauth2/callback" },
                    RequireConsent = false,
                    PostLogoutRedirectUris = { $"https://www.getpostman.com" },
                    AllowedCorsOrigins =     { $"https://www.getpostman.com" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "basket",
                        "orders",
                        "webshoppingagg"
                    }
                }
            };
        }
    }
}