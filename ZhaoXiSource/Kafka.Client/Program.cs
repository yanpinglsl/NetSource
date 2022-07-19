using System;
using System.Collections.Generic;

namespace Kafka.Consume
{
    class Program
    {
        static void Main(string[] args)
		{
			//.net coer 看看源代码
			//FileStream fileStream = new FileStream("", FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
			//fileStream.Write(Encoding.UTF8.GetBytes("abc"));


			//StreamWriter streamWriter = new StreamWriter(fileStream);

			//StreamReader streamread= new StreamReader(fileStream);
			// 然而啥都有
			//java 
			// 文件流,字节流,字符流// socket 流//通过网络,实现通讯
			// iso websocket >http的性能
			//netty 200 
			//kafka  netty
			//es  netty
			//NetKafka.Pull();
			Console.WriteLine("Hello World!");
			string brokerList = "120.78.170.106:9092,120.78.170.106:9093,120.78.170.106:9094";//,39.96.82.51:9093
																						   //string brokerList = "localhost:9092"; // 

			//原理+实战
			var topics = new List<string> { "test" };
			Console.WriteLine("请输入组名称");
			string groupname = Console.ReadLine();
			ConfulentKafka.Consumer(brokerList, topics, groupname);
		}
    }
}
