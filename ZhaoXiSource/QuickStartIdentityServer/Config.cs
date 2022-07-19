using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickStartIdentityServer
{
    public class Config
    {
        /// <summary>
        /// 用户认证信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                    new IdentityResources.OpenId(),//添加对标准 openid（subject id）的支持
                    new IdentityResources.Profile(),//添加对标准profile （名字，姓氏等）Scope的支持
                                                    //new IdentityResources.Address(),
                                                    //new IdentityResources.Email(),
                                                    //new IdentityResources.Phone()
            };
        }

        /// <summary>
        /// API资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
                {
                    new ApiResource("api1", "My API"){ Scopes={ "group1" } },
                    new ApiResource("api2", "My API"){ Scopes={ "group1" } }
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

        /// <summary>
        /// 客户端应用
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
                {
                    #region 客户端模式授权
                    new Client
                    {
                        //客户端ID
                        ClientId = "client",

                        //AccessToken 过期时间，默认3600秒，注意这里直接设置5秒过期是不管用的，解决方案继续看下面 API资源添加JWT
                        //AccessTokenLifetime=5,

                        //配置授权类型，可以配置多个授权类型
                        //没有交互性用户，使用 clientid/secret 实现认证。
                        AllowedGrantTypes = GrantTypes.ClientCredentials,

                        // 客户端加密方式
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },

                        //配置授权范围，这里指定哪些API 受此方式保护
                        //AllowedScopes = { "api1" }
                        AllowedScopes = { "group1" }
                    },
                    #endregion
                    #region 资源所有者密码授权模式
                    new Client
                    {
                        ClientId = "ro.client",
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        //AllowedScopes = { "api1" }
                        AllowedScopes = {
                            "group1"
                        }
                    }
                    #endregion
                    #region 授权码模式
                    ////如果不使用https请求则需要进行额外处理（http 在谷歌浏览器运行会导致跳转失败）
                    //new Client
                    // {
                    //     ClientId = "mvc",
                    //     ClientName = "MVC Client",
                    //     ClientSecrets = { new Secret("123456".Sha256()) },
                    //     AllowedGrantTypes = GrantTypes.Code,
                    //     //需要确认授权
                    //     //RequireConsent = true,
                    //     //RequirePkce = true, 
                    //     //允许token通过浏览器
                    //    AllowAccessTokensViaBrowser=true, 
                    //    // 登录成功回调处理地址，处理回调返回的数据
                    //    RedirectUris = { "https://localhost:5002/signin-oidc" },//5002是Mvc客户端的端口signin-oidc是IDS4监听的一个地址,可以拿到token信息

                    //    // 退出登录回调处理地址
                    //    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },
                    //    //允许的范围
                    //    AllowedScopes = new List<string>
                    //    {
                    //        IdentityServerConstants.StandardScopes.OpenId,
                    //        IdentityServerConstants.StandardScopes.Profile
                    //    },
                    //    AlwaysIncludeUserClaimsInIdToken=true
                    //}
                    #endregion
                };
        }

        #region 资源所有者密码授权模式
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "1",
                        Username = "alice",
                        Password = "password",

                        Claims = new []
                        {
                            new Claim("name", "Alice"),
                            new Claim("website", "https://alice.com")
                        }
                    },
                    new TestUser
                    {
                        SubjectId = "2",
                        Username = "bob",
                        Password = "password",

                        Claims = new []
                        {
                            new Claim("name", "Bob"),
                            new Claim("website", "https://bob.com")
                        }
                    }
                };
        }
        #endregion

    }
}
