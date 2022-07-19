using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Server.Entities
{
	public class DeptinfoEntities
	{
		public List<DeptInfo> DeptInfos;
		public DeptinfoEntities()
		{
			DeptInfos = new List<DeptInfo>();
			DeptInfos.Add(new DeptInfo()
			{
				DeptId = 1,
				Name = "研发部"
			});
			DeptInfos.Add(new DeptInfo()
			{
				DeptId = 2,
				Name = "法务部"
			});
			DeptInfos.Add(new DeptInfo()
			{
				DeptId = 3,
				Name = "业务部"
			});
		}
	}
}
