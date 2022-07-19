using DotNetCore.CAP;
using EFModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private static string PublishName = "RabbitMQ.SQLServer.UserService";

        private readonly IConfiguration _iConfiguration;
        /// <summary>
        /// 构造函数注入---默认IOC容器完成---注册是在AddCAP
        /// </summary>
        private readonly ICapPublisher _iCapPublisher;
        private readonly CommonServiceDbContext _UserServiceDbContext;
        private readonly ILogger<UserController> _Logger;

        public UserController(ICapPublisher capPublisher, IConfiguration configuration, CommonServiceDbContext userServiceDbContext, ILogger<UserController> logger)
        {
            this._iCapPublisher = capPublisher;
            this._iConfiguration = configuration;
            this._UserServiceDbContext = userServiceDbContext;
            this._Logger = logger;
        }
        //{"Headers":{"cap-callback-name":null,"cap-msg-id":"1403333028973604864","cap-corr-id":"1403333028973604864","cap-corr-seq":"0","cap-msg-name":"RabbitMQ.SQLServer.UserService","cap-msg-type":"User","cap-senttime":"2021/6/11 20:47:21 \u002B08:00"},"Value":{"Id":1,"Name":"\u81EA\u95ED\u75C7\u60A3\u80052021","Account":"admin","Password":"e10adc3949ba59abbe56e057f20f883e","Email":"12","Mobile":"133","CompanyId":1,"CompanyName":"\u767E\u6377","State":0,"UserType":2,"LastLoginTime":"2015-12-12T00:00:00","CreateTime":"2015-12-12T00:00:00","CreatorId":1,"LastModifierId":1,"LastModifyTime":"2019-04-11T10:48:43"}}

        [Route("/without/transaction")]//根目录
        public async Task<IActionResult> WithoutTransaction()
        {
            //var user = this._UserServiceDbContext.User.FirstOrDefault();//这种写法会报错
            var user = this._UserServiceDbContext.User.FirstOrDefault();
            this._Logger.LogWarning($"This is WithoutTransaction Invoke");
            await _iCapPublisher.PublishAsync(PublishName, user);//应该把数据写到publish表
            return Ok();
        }

        [Route("/adotransaction/sync")]//根目录
        public IActionResult AdoTransaction()
        {
            var user = this._UserServiceDbContext.User.FirstOrDefault();
            IDictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("Teacher", "Eleven");
            dicHeader.Add("Student", "Seven");
            dicHeader.Add("Version", "1.2");

            using (var connection = new SqlConnection(this._iConfiguration.GetConnectionString("UserServiceConnection")))
            {
                using (var transaction = connection.BeginTransaction(this._iCapPublisher, true))
                {
                    //user.Name += "2021";
                    //this._UserServiceDbContext.SaveChanges();
                    _iCapPublisher.Publish(PublishName, user, dicHeader);//带header
                }
            }
            this._Logger.LogWarning($"This is AdoTransaction Invoke");
            return Ok();
        }

        [Route("/efcoretransaction/async")]//根目录
        public IActionResult EFCoreTransaction()
        {
            var user = this._UserServiceDbContext.User.FirstOrDefault();//读个数据
            var userNew = new User()
            {
                Name = "Eleven" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                CompanyId = 1,
                CompanyName = "朝夕教育",
                CreateTime = DateTime.Now,
                CreatorId = 1,
                LastLoginTime = DateTime.Now,
                LastModifierId = 1,
                LastModifyTime = DateTime.Now,
                Password = "123456",
                State = 1,
                Account = "Administrator",
                Email = "57265177@qq.com",
                Mobile = "18664876677",
                UserType = 1
            };//new个对象

            IDictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("Teacher", "Eleven");
            dicHeader.Add("Student", "Seven");
            dicHeader.Add("Version", "1.2");
            //完成 业务+publish的本地事务
            using (var trans = this._UserServiceDbContext.Database.BeginTransaction(this._iCapPublisher, autoCommit: false))
            {
                this._UserServiceDbContext.User.Add(userNew);//数据库插入对象
                this._UserServiceDbContext.SaveChanges();//提交---Context事务的

                _iCapPublisher.Publish(PublishName, user, dicHeader);//带header
                //publish做的就只是把数据写入到publish表

                //throw new Exception();就都写不进去了

                Console.WriteLine("数据库业务数据已经插入");
                trans.Commit();
            }
            this._Logger.LogWarning($"This is EFCoreTransaction Invoke");
            return Ok("Done");
        }

        #region 多节点贯穿协作
        [Route("/Distributed/Demo/{id}")]//根目录aaa
        public IActionResult Distributed(int? id)
        {
            int index = id ?? 11;
            string publishName = "RabbitMQ.SQLServer.DistributedDemo.User-Order";

            var user = this._UserServiceDbContext.User.FirstOrDefault();
            var userNew = new User()
            {
                Name = "Eleven" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"),
                CompanyId = 1,
                CompanyName = "朝夕教育" + index,
                CreateTime = DateTime.Now,
                CreatorId = 1,
                LastLoginTime = DateTime.Now,
                LastModifierId = 1,
                LastModifyTime = DateTime.Now,
                Password = "123456" + index,
                State = 1,
                Account = "Administrator" + index,
                Email = "57265177@qq.com",
                Mobile = "18664876677",
                UserType = 1
            };

            IDictionary<string, string> dicHeader = new Dictionary<string, string>();
            dicHeader.Add("Teacher", "Eleven");
            dicHeader.Add("Student", "Seven");
            dicHeader.Add("Version", "1.2");
            dicHeader.Add("Index", index.ToString());

            using (var trans = this._UserServiceDbContext.Database.BeginTransaction(this._iCapPublisher, autoCommit: false))
            {
                this._UserServiceDbContext.User.Add(userNew);
                this._iCapPublisher.Publish(publishName, user, dicHeader);//带header
                this._UserServiceDbContext.SaveChanges();
                Console.WriteLine("数据库业务数据已经插入");
                trans.Commit();
            }
            this._Logger.LogWarning($"This is EFCoreTransaction Invoke");
            return Ok("Done");
        }

        #endregion

    }
}
