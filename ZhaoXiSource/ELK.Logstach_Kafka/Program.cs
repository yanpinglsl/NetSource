using System;

namespace ELK.Logstach_Kafka
{
    class Program
    {
        static  void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			while (1 == 1)
			{
				Console.WriteLine("请输入发送的内容");
				var message = Console.ReadLine();
				string brokerList = "120.78.170.106:9092";
				 ConfulentKafka.Produce(brokerList, "kafkalog", message);
			}
		}
    }
}
