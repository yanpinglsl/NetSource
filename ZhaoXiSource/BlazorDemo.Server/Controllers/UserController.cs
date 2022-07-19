using BlazorDemo.Server.Paging;
using BlazorDemo.Server.Service;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserinfoService UserinfoService;

        public UserController(IUserinfoService userinfoService)
        {
            UserinfoService = userinfoService;
        }

        [HttpGet]
        [Route("GetAll/{deptid}")]
        public IList<Userinfo> GetAllForDepartment
           (int deptid)
        {
            return UserinfoService.GetForDepartment(new UserParameters());
        }
        [HttpGet]
        [Route("GetUser/{userid}")]
        public Userinfo GetUser(int userid)
        {
            return UserinfoService.GetForDepartment(new UserParameters()).Where(m => m.UserID == userid).SingleOrDefault();
        }

        [HttpPost]
        [Route("AddUser")]
        public Userinfo AddUser(Userinfo userinfo)
        {
            return UserinfoService.Add(userinfo);
        }

        [HttpPost]
        [Route("UpdateUser")]
        public Userinfo UpdateUser(Userinfo userinfo)
        {
            UserinfoService.Update(userinfo);
            return userinfo;
        }

        [HttpDelete]
        [Route("DeleteUser/{userid}")]
        public ActionResult DeleteUser(int userid)
        {
            UserinfoService.Delete(userid);
            return Ok();
        }
        [HttpGet]
        [Route("GetPage")]
        public PagedList<Userinfo> GetUserinfosPage([FromQuery] UserParameters userParameters)
        {
            //await _context.Products
            // .Search(productParameters.SearchTerm)
            // .Sort(productParameters.OrderBy)
            // .ToPagedListAsync(productParameters.PageNumber, productParameters.PageSize);
            var users = UserinfoService.GetForDepartment(userParameters).ToPagedList(userParameters.PageNumber, userParameters.PageSize);
            // 计算总共多少页，总条数，，把这数据打包封装
            // 不想给返回值包装跟多的参数，直接把这数据通过http 头部传递、、 
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(users.MetaData));

            return users;

        }
    }
}
