using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zhaoxi.MongodbApp
{
	public class Transaction
	{
		static object o = new object();
		public static void Show()
		{
 


			//MongoClient client = new MongoClient("mongodb://localhost:30001,localhost:30002,localhost:30003");
			MongoClient client = new MongoClient("mongodb://39.96.34.52:27017,47.95.2.2:27017,39.96.82.51:27017");

			//MongoClient client = new MongoClient("mongodb://localhost:30002");
			//事务
			var session = client.StartSession();
			var database = session.Client.GetDatabase("test");
			session.StartTransaction(new TransactionOptions(
				readConcern: ReadConcern.Snapshot,
				writeConcern: WriteConcern.WMajority));
			try
			{
				IMongoCollection<Userinfo> collection = database.GetCollection<Userinfo>("userinfo");
				IMongoCollection<DetpInfo> weiguocollection = database.GetCollection<DetpInfo>("deptindo");
				Userinfo daqiao = new Userinfo()
				{
					Id = Guid.NewGuid().ToString(),
					Address = "吴国",
					Name = "大乔",
					Sex = "女",
					DetpInfo = new DetpInfo()
					{
						DeptId = 1,
						DeptName = "蜀国集团"
					}
				};
			 
				collection.InsertOne(session, daqiao);
				//throw new Exception("取消事务");
				
			    DetpInfo weiguo = new DetpInfo() { DeptId = 1, DeptName = "魏国" };
				weiguocollection.InsertOne(session,weiguo);
				session.CommitTransaction();
			}
			catch (Exception ex)
			{
				//回滚
				session.AbortTransaction();
				Console.WriteLine(ex.Message);
			} 

			Console.WriteLine("ok");
		}
	}
}
