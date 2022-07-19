using MongoDBCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest
{
	public class MysqlTest
	{

		public static void Show()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			string sql = @"INSERT into userinfo(userid, username, address,age)
                         values('12345-userid', 'username', 'addressaddressaddressaddressaddressaddressaddressaddress',18);";

			var sqls = "";
			for (int i = 0; i < 300; i++)
			{
				sqls += sql;
			}


			// 开启了三十个线程，每一个线程执行100条sql

			// 做压力测试
			List<Task> tasks = new List<Task>();
			Parallel.For(1, 200, (i) =>
			{

				tasks.Add(Task.Run(() =>
				{
					try
					{
						var k = MySqlHelper.ExecuteNonQuery(MySqlHelper.Conn, System.Data.CommandType.Text, sqls);
						Console.WriteLine(k);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
						throw;
					}
				}));


			});
			Task.WaitAll(tasks.ToArray());
			stopwatch.Stop();
			Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
			Console.WriteLine("ok");
		}
	}
}
