using System;
using System.Threading.Tasks;

namespace Kafka.Produce
{
    class Program
    {
        static async Task Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			while (1 == 1)
			{

				Console.WriteLine("请输入发送的内容");
				var message = Console.ReadLine();
				//NetKafka.Push("mytopic1", message);
				string brokerList = "120.78.170.106:9092,120.78.170.106:9093,120.78.170.106:9094";
                //string brokerList = "39.96.82.51:9093,47.95.2.2:9092,39.96.34.52:9092";
                //string brokerList = "localhost:9092";
                await ConfulentKafka.Produce(brokerList, "test", message);
			}
		}
    }
}
