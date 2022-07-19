using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerApp.Utitly
{/// <summary>
 /// 客户端模式
 /// </summary>
    public class ClientInitConfig
    {
        /// <summary>
        /// 定义ApiResource   
        /// 这里的资源（Resources）指的就是管理的API
        /// </summary>
        /// <returns>多个ApiResource</returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
            new ApiResource("UserApi", "用户获取API")
        };
        }

        /// <summary>
        /// 定义验证条件的Client
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
            new Client
            {
                ClientId = "MengLin.Shopping.Web",//客户端惟一标识
                ClientSecrets = new [] { new Secret("MengLin123456".Sha256()) },//客户端密码，进行了加密
                AllowedGrantTypes = GrantTypes.ClientCredentials,//Grant类型
                AllowedScopes = new [] { "UserApi" },//允许访问的资源
                Claims= new List<Claim>()
                {
                    new Claim(IdentityModel.JwtClaimTypes.Role,"Admin"),
                    new Claim(IdentityModel.JwtClaimTypes.NickName,"豆豆爸爸"),
                    new Claim("EMail","menglin2010@126.com")
                }
            }
        };
        }
    }
}
