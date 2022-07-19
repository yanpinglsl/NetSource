using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DemoProject.Controllers
{
    public class ConfigurationController : Controller
    {
        private readonly IConfiguration _iConfiguration = null;
        public ConfigurationController(IConfiguration configuration)
        {
            this._iConfiguration = configuration;
        }

        /// <summary>
        /// http://localhost:5726/Configuration/Index
        /// appsetting.json
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            string AllowedHosts = this._iConfiguration["AllowedHosts"];
            //string allowedHost = this._iConfiguration["AllowedHost"].ToString();//异常

            string today = this._iConfiguration["Today"];
            string writeConn = this._iConfiguration["ConnectionStrings:Write"];
            string readConn0 = this._iConfiguration["ConnectionStrings:Read:0"];
            string[] _SqlConnectionStringRead = this._iConfiguration.GetSection("ConnectionStrings").GetSection("Read").GetChildren().Select(s => s.Value).ToArray();

            Console.WriteLine($"AllowedHosts={AllowedHosts}");
            Console.WriteLine($"today={today}");
            Console.WriteLine($"writeConn={writeConn}");
            Console.WriteLine($"readConn0={readConn0}");
            Console.WriteLine($" _SqlConnectionStringRead={string.Join(",", _SqlConnectionStringRead)}");

            return View();
        }

        /// <summary>
        /// dotnet run --urls="http://*:5726" ip="127.0.0.1" /port=5726 ConnectionStrings:Write=CommandLineArgument
        /// </summary>
        /// <returns></returns>
        public IActionResult CommandLine()
        {
            /*
             var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddCommandLine(args)
            .Build();
             */
            string urls = this._iConfiguration["urls"];
            string ip = this._iConfiguration["ip"];
            string port = this._iConfiguration["port"];
            string writeConn = this._iConfiguration["ConnectionStrings:Write"];

            Console.WriteLine($"urls={urls} ip={ip} port={port} writeConn={writeConn} ");

            return View();
        }

        public IActionResult Bind()
        {
            RabbitMQOptions rabbitMQOptions1 = new RabbitMQOptions();
            this._iConfiguration.GetSection("RabbitMQOptions").Bind(rabbitMQOptions1);
            Console.WriteLine($"HostName={rabbitMQOptions1.HostName}");

            RabbitMQOptions rabbitMQOptions2 = this._iConfiguration.GetSection("RabbitMQOptions").Get<RabbitMQOptions>();
            Console.WriteLine($"HostName2={rabbitMQOptions2.HostName}");

            return View();
        }

        public class RabbitMQOptions
        {
            public string HostName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }

        public IActionResult Memory()
        {
            string HostName = this._iConfiguration["RabbitMQOptions:HostName"];
            string TodayMemory = this._iConfiguration["TodayMemory"];
            Console.WriteLine($"HostName={HostName}");
            Console.WriteLine($"TodayMemory={TodayMemory}");

            return View();
        }


        public IActionResult XML()
        {
            string AllowedHosts = this._iConfiguration["AllowedHosts"];
            string HostName = this._iConfiguration["RabbitMQOptions:HostName"];
            string TodayXML = this._iConfiguration["TodayXML"];
            Console.WriteLine($"AllowedHosts={AllowedHosts}");
            Console.WriteLine($"HostName={HostName}");
            Console.WriteLine($"TodayXML={TodayXML}");

            return View();
        }


        public IActionResult Custom()
        {
            Console.WriteLine($"TodayCustom={this._iConfiguration["TodayCustom"]}");
            Console.WriteLine($"HostName={this._iConfiguration["RabbitMQOptions-Custom:HostName"]}");
            Console.WriteLine($"UserName={this._iConfiguration["RabbitMQOptions-Custom:UserName"]}");
            Console.WriteLine($"Password={this._iConfiguration["RabbitMQOptions-Custom:Password"]}");
            Console.WriteLine($"LogTagCustom={this._iConfiguration["RabbitMQOptions-Custom:LogTag"]}");

            return View();
        }
    }
}
