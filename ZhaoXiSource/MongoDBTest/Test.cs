using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet;
using BenchmarkDotNet.Attributes;
using MongoDBCommon;

namespace MongoDBTest
{
	public class Test
	{
		//[Params(100,100)]
		public int A { get; set; } = 20;

		[Benchmark]
		public void Mysql()
		{
			for (int i = 0; i < this.A; i++)
			{
				string sql = @"INSERT into userinfo(userid, username, address,age)
                         values('12345-userid', 'username', 'addressaddressaddressaddressaddressaddressaddressaddress',18);";
				var k = MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, System.Data.CommandType.Text, sql);
			}

		}
		[Benchmark]
		public void Mongodb()
		{
			for (int i = 0; i < this.A; i++)
			{
				var userinfo = new Userinfo()
				{
					UserId = "12345-userid",
					UserName = "username",
					Address = "addressaddressaddressaddressaddressaddressaddressaddress",
					Age = 18
				};
				MongoDbHelper<Userinfo> mongoDbHelper = new MongoDbHelper<Userinfo>();
				mongoDbHelper.InsertOne(userinfo);
			}
		}
	}
}
