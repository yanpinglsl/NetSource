using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YY.AgileFramework.Common.Models;
using YY.MSACommerce.DTOModel.DTO;

namespace YY.MSACormmerce.AuthenticationCenter.Utility
{
    public class HttpHelperService
    {
        /// <summary>
        /// 调用校验服务
        /// </summary>
        /// <param name="userUrl"></param>
        /// <returns></returns>
        public AjaxResult<DTOJWTUser> VerifyUser(string userUrl)
        {
            AjaxResult<DTOJWTUser> ajaxResult = null;
            HttpResponseMessage sResult = this.HttpRequest(userUrl, HttpMethod.Get, null);
            if (sResult.IsSuccessStatusCode)
            {
                string content = sResult.Content.ReadAsStringAsync().Result;
                ajaxResult = Newtonsoft.Json.JsonConvert.DeserializeObject<AjaxResult<DTOJWTUser>>(content);
            }
            else
            {
                ajaxResult = new AjaxResult<DTOJWTUser>()
                {
                    StatusCode = (int)sResult.StatusCode,
                    Result = false,
                };
            }
            return ajaxResult;
        }

        public HttpResponseMessage HttpRequest(string url, HttpMethod httpMethod, Dictionary<string, string> parameter)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpRequestMessage message = new HttpRequestMessage()
                {
                    Method = httpMethod,
                    RequestUri = new Uri(url)
                };
                if (parameter != null)
                {
                    var encodedContent = new FormUrlEncodedContent(parameter);
                    message.Content = encodedContent;
                }
                return httpClient.SendAsync(message).Result;
            }
        }


    }
}
