using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zhaoxi.MongodbApp
{
	public class LinqOperation
	{
		static DBbase<Userinfo> dBbase = new DBbase<Userinfo>();

		public static void DropDatabase()
		{
			dBbase.DropDatabase();
		}
		public static void InserOne()
		{


			dBbase.InsertOne(new Userinfo()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "诸葛亮",
				Address = "蜀国",
				Age = 27,
				Sex = "男",
				DetpInfo = new DetpInfo()
				{
					DeptId = 1,
					DeptName = "蜀国集团"
				}
			});
			dBbase.InsertOne(new Userinfo()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "荀彧",
				Address = "魏国",
				Age = 47,
				Sex = "男",
				DetpInfo = new DetpInfo()
				{
					DeptId = 2,
					DeptName = "魏国集团"
				}
			});
			Console.WriteLine("写入完成");
		}

		public static void InsertMany()
		{

			var zhouyu = new Userinfo()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "周瑜",
				Address = "吴国",
				Age = 32,
				Sex = "男",
				DetpInfo = new DetpInfo() { DeptId = 3, DeptName = "吴国集团" }
			};
			var daqiao = new Userinfo()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "大乔",
				Address = "吴国",
				Age = 14,
				Sex = "女",
				DetpInfo = new DetpInfo() { DeptId = 3, DeptName = "吴国集团" }
			};
			var caocao = new Userinfo()
			{
				Id = Guid.NewGuid().ToString(),
				Name = "曹操",
				Address = "魏国",
				Age = 32,
				Sex = "男",
				DetpInfo = new DetpInfo() { DeptId = 3, DeptName = "魏国集团" }
			};
			dBbase.InsertMany(zhouyu, daqiao, caocao);
			Console.WriteLine("批量写入完成");
		}

		public static void GetPage()
		{

			var pagelist = dBbase.Select(m => m.Sex == "男", m => m.Age, 2, 2);
			foreach (var item in pagelist)
			{
				Console.WriteLine(item.Name + ":" + item.Age);
			}

			var query = from p in dBbase.collection.AsQueryable()
						where (p.Sex == "男") && p.Age > 18
						select p;
			foreach (var item in query)
			{
				Console.WriteLine(item.Name + ":" + item.Age);
			}
		}

		public static void Update()
		{
			var daqiaod = dBbase.Select().Where(m => m.Name == "大乔").FirstOrDefault();
			daqiaod.Age = 18;
			dBbase.UpdateOne(m => m.Id == daqiaod.Id, daqiaod);
			Console.WriteLine("修改完成");
		}
		public static void DeleteMany()
		{
			dBbase.DeleteMany(m => m.Name == "大乔");
			Console.WriteLine("删除完成");
			//全部删除
			dBbase.DeleteMany(m => 1 == 1);
			Console.WriteLine("全部删除完成");


		}
		public static void GroupBy()
		{

			//linq  
			var groups = dBbase.collection.AsQueryable().GroupBy(m => new { m.DetpInfo.DeptId, m.DetpInfo.DeptName }).Select(t => new
			{
				DeptId = t.Key.DeptId,
				DeptName = t.Key.DeptName,
				number = t.Count(),
				ages = t.Sum(s => s.Age)
			}).Take(0).Skip(10);
			foreach (var item in groups)
			{
				Console.WriteLine(item.DeptName + ":" + item.number + ":" + item.ages);
			}
		}
	}
}
