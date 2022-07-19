using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROP.IdentityServer
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API"){ Scopes={ "group1" } },
                new ApiResource("api2", "My API"){ Scopes={ "group1","group2" } }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                //new ApiScope("api1")
                new ApiScope("group1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                     //配置授权类型，可以配置多个授权类型
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,  

                    // 客户端加密方式
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    //配置授权范围，这里指定哪些API 受此方式保护
                    //AllowedScopes = { "api1" }
                    AllowedScopes = { "group1" }
                }
            };
        }
    }
}
