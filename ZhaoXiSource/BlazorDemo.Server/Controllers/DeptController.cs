using BlazorDemo.Server.Service;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Server.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class DeptController : ControllerBase
	{
		private IDeptinfoService DeptinfoService;

		public DeptController(IDeptinfoService deptinfoService)
		{
			DeptinfoService = deptinfoService;
		}

		[HttpGet]
		[Route("GetAll")]
		public IList<DeptInfo> GetAllForDepartment()
		{
			return DeptinfoService.GetAll();
		}

	}
}
