using BenchmarkDotNet.Running;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDBTest
{
	class Program
	{
		static void Main(string[] args)
		{
			 

			//var client = new MongoClient("mongodb://39.96.34.52:27017");
			//var database = client.GetDatabase("test");
			//var document = BsonDocument.Parse("{ a: 1, b: [{ c: 1 }],c: 'ff'}");
			//var document1 = BsonDocument.Parse("{ a: 6666}");
			//database.GetCollection<BsonDocument>("userinfo").InsertOne(document);

			BenchmarkRunner.Run<Test>();

			//MysqlTest.Show();
			//MongodbTest.Show();  
			Console.ReadKey();
		}
	}
}
