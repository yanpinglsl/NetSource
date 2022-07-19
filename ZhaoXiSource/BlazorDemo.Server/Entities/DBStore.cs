using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Server.Entities
{
	public class DBStore
	{
		public static List<DeptInfo> Deptinfo = new DeptinfoEntities().DeptInfos;

		public static List<Userinfo> Userinfo = new UserinfoEntities().Userinfos;
	}
}
