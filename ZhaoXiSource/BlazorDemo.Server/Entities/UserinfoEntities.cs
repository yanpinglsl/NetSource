using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Server.Entities
{
	public class UserinfoEntities
	{
		public List<Userinfo> Userinfos;
		public UserinfoEntities()
		{
			Userinfos = new List<Userinfo>();
			
			for (int i = 0; i < 100; i++)
			{
				Userinfo userinfo = new Userinfo();
				Userinfos.Add(userinfo);
				userinfo.Address = "中华人民共和国" + i;
				userinfo.Age = i;
				userinfo.BirthDate = DateTime.UtcNow;
				userinfo.Gender = Gender.Male;
				userinfo.UserID = (i + 1);
				userinfo.UserName = "无名氏" + i;
				userinfo.DeptId = 1;
				if (i % 2 == 0)
				{
					userinfo.DeptId = 2;
				}
				if (i % 3 == 0)
				{
					userinfo.DeptId = 3;
				}

			}
		}
	}
}
