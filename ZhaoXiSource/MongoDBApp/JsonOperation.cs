using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zhaoxi.MongodbApp
{
	public class JsonOperation
	{
		public static void Show()
		{
			//var client = new MongoClient("mongodb://192.168.2.202:27017");
			//var database = client.GetDatabase("test");
			//var document = BsonDocument.Parse("{ a: 1, b: [{ c: 1 }],c: 'ff'}");
			//var document1 = BsonDocument.Parse("{ a: 6666}");
			//database.GetCollection<BsonDocument>("userinfo").InsertOne(document1);

			//database.GetCollection<BsonDocument>("userinfo").InsertOne(document);

			var client = new MongoClient("mongodb://192.168.3.202:27017");
			var database = client.GetDatabase("mongodbDemo");
			var document = BsonDocument.Parse("{ a: 1, b: [{ c: 1 }],c: 'ff'}");
			database.GetCollection<BsonDocument>("order").InsertOne(document);
			Console.WriteLine("ok");

		}
	}
}
