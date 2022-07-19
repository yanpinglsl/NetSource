using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoProject.Controllers
{
    //类名以Controller结尾，或者标记Controller特性，该类就会被当作控制器加载
    public class CTestAController
    {
        public IActionResult Index()
        {
            return new JsonResult(new { ID = 1, Name = "AAAA" });
        }
    }
    //[Controller]
    //public class CTestA
    //{
    //    public IActionResult Index()
    //    {
    //        return new JsonResult(new { ID = 1, Name = "AAAA" });
    //    }
    //}
}
