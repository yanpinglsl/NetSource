
using BlazorDemo.Server.Entities;
using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Server.Service
{
	public class DeptinfoService: IDeptinfoService
	{
		public List<DeptInfo> GetAll()
		{
			return DBStore.Deptinfo;
		}
		public DeptInfo GetById(int deptid)
		{
			return DBStore.Deptinfo.SingleOrDefault(m => m.DeptId == deptid);
		}

	}
}
