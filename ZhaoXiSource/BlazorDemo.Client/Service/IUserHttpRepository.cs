using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Service
{
	public interface IUserHttpRepository
	{
		public Task<Userinfo> GetUserinfosById(int userid);
		public Task<List<Userinfo>> GetUserinfos();
		public Task<PagingResponse<Userinfo>> GetUserinfos(UserParameters userParameters);
		public Task<List<DeptInfo>> GetDeptInfos();
		public Task<Userinfo> AddUserinfo(Userinfo userinfo);
		public Task<Userinfo> UpdateUser(Userinfo userinfo);
		public Task<bool> DeleteUser(string userid);
		public Task<string> UploadFile(MultipartFormDataContent content);
	}
}
