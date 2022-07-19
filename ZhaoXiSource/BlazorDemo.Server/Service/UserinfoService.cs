using BlazorDemo.Server.Entities;
using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Server.Service
{
	public class UserinfoService : IUserinfoService
	{
		public List<Userinfo> GetForDepartment(UserParameters userParameters)
		{

			var userinfos = DBStore.Userinfo;
			if (!string.IsNullOrEmpty(userParameters?.SearchTerm))
			{
				userinfos = DBStore.Userinfo.Where(m => m.UserName.Contains(userParameters.SearchTerm)).ToList();
			}

			return userinfos;
		}

		public Userinfo GetOne(int deptid, int id)
		{
			return DBStore.Userinfo.SingleOrDefault(m => m.DeptId == deptid && m.UserID == id);
		}

		public Userinfo Add(Userinfo userinfo)
		{
			userinfo.UserID = DBStore.Userinfo.Count;
			DBStore.Userinfo.Add(userinfo);
			return userinfo;
		}
		public void Update(Userinfo userinfo)
		{
			var user = DBStore.Userinfo.SingleOrDefault(m => m.UserID == userinfo.UserID);
			DBStore.Userinfo.Remove(user);
			DBStore.Userinfo.Add(userinfo);
		}

		public void Delete(int id)
		{
			var exist = DBStore.Userinfo.SingleOrDefault(m => m.UserID == id);
			if (exist == null)
			{
				throw new Exception("未能找到该用户");
			}
			DBStore.Userinfo.Remove(exist);
		}
	}
}
