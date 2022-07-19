using System;

namespace StackExchangeRedis
{
	class Program
	{
		static void Main(string[] args)
		{
			////127.0.0.1:6379,127.0.0.1:6380,127.0.0.1:6381,127.0.0.1:6382,password=123456,abortConnect=false
			RedisHelper redis = new RedisHelper("127.0.0.1:7000,127.0.0.1:7001,127.0.0.1:7002,127.0.0.1:7003,127.0.0.1:7004,127.0.0.1:7005");
			redis.StringSet("yyy", "clay666");
			Console.WriteLine(redis.StringGet("yyy"));
		}
	}
}
