using ServiceStack.Redis;
using System;

namespace ELK.Logstach_Redis
{
    class Program
    {
        static void Main(string[] args)
		{
            try
			{
				Console.WriteLine("Hello World!");
				string listkey = "listlog";
				while (1 == 1)
				{
					Console.WriteLine("请输入发送的内容");
					var message = Console.ReadLine();
					using (RedisClient client = new RedisClient("192.168.200.104",6379))
					{
						client.AddItemToList(listkey, message);
					}
				}

			}
			catch(Exception ex)
            {
				Console.WriteLine(ex.Message);
            }
		}
    }
}
