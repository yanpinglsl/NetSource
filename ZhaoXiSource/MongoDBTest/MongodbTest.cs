using MongoDBCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MongoDBTest
{


	public class Userinfo
	{

		public string UserId { get; set; }
		public string UserName { get; set; }
		public string Address { get; set; }

		public int Age { get; set; }

	}

	public class MongodbTest
	{

		public static void Show()
		{

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			List<Userinfo> userinfos = new List<Userinfo>();
			for (int i = 0; i < 2; i++)
			{
				userinfos.Add(new Userinfo()
				{
					UserId = "12345-userid",
					UserName = "username",
					Address = "addressaddressaddressaddressaddressaddressaddressaddress",
					Age=18
				});
			}
			List<Task> tasks = new List<Task>();
			Parallel.For(1, 2, (i) =>
			{

				tasks.Add(Task.Run(() =>
				{
					try
					{
						MongoDbHelper<Userinfo> mongoDbHelper = new MongoDbHelper<Userinfo>(); 
						mongoDbHelper.InsertMany(userinfos.ToArray());
						Console.WriteLine(300);
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
