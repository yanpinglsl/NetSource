using BlazorDemo.Shared;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Service
{
    public class UserHttpRepository : IUserHttpRepository
    {
        private readonly HttpClient _client;
        public UserHttpRepository(HttpClient client)
        {
            _client = client;
        }
        public async Task<List<Userinfo>> GetUserinfos()
        {
            var response = await _client.GetAsync("user/GetAll/1");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Userinfo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Userinfo> AddUserinfo(Userinfo userinfo)
        {
            var userinfoJson =
                           new StringContent(
                               JsonSerializer.Serialize(userinfo),
                               Encoding.UTF8,
                               "application/json");

            var response = await _client.PostAsync(
                "user/AddUser", userinfoJson);

            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<Userinfo>
                    (await response.Content.ReadAsStreamAsync());
            }

            return null;
        }

        public async Task<bool> DeleteUser(string userid)
        {
            var response = await _client.DeleteAsync("user/DeleteUser/" + userid);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<List<DeptInfo>> GetDeptInfos()
        {
            var response = await _client.GetAsync("dept/GetAll/");
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DeptInfo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        public async Task<PagingResponse<Userinfo>> GetUserinfos(UserParameters userParameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = userParameters.PageNumber.ToString(),
                ["searchTerm"] = userParameters.SearchTerm ?? "",
            };

            //Microsoft.AspNetCore.WebUtilities
            var response = await _client.GetAsync(QueryHelpers.AddQueryString("user/GetPage", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pagingResponse = new PagingResponse<Userinfo>
            {
                Items = JsonSerializer.Deserialize<List<Userinfo>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            };

            return pagingResponse;
        }

        public async Task<Userinfo> GetUserinfosById(int userid)
        {
            var response = await _client.GetAsync("user/GetUser/" + userid);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Userinfo>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<Userinfo> UpdateUser(Userinfo userinfo)
        {
            var userinfoJson =
                            new StringContent(
                                JsonSerializer.Serialize(userinfo),
                                Encoding.UTF8,
                                "application/json");

            var response = await _client.PostAsync(
                "user/UpdateUser", userinfoJson);
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<Userinfo>
                    (await response.Content.ReadAsStreamAsync());
            }

            return null;
        }

        public Task<string> UploadFile(MultipartFormDataContent content)
        {
            throw new NotImplementedException();
        }
    }
}
