
using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BlazorDemo.Server.Service
{
	public interface IUserinfoService
	{

		public List<Userinfo> GetForDepartment(UserParameters userParameters);

		public Userinfo GetOne(int deptid, int id);

		public Userinfo Add(Userinfo userinfo);
		public void Update(Userinfo userinfo);

		public void Delete(int id);
	}
}
