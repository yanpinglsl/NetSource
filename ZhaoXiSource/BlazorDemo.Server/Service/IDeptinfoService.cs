
using BlazorDemo.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BlazorDemo.Server.Service
{
	public interface IDeptinfoService
	{
		public List<DeptInfo> GetAll();
		public DeptInfo GetById(int deptid);
	}
}
