using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MongoDBCommon
{

	public class MongoDbHelper<T> where T : class, new()
	{
		MongoClient client;
		IMongoDatabase database;
		public IMongoCollection<T> collection;
		public MongoDbHelper()
		{
			//var client = new MongoClient("mongodb://host:27017,host2:27017/?replicaSet=rs0");
			client = new MongoClient("mongodb://39.96.34.52:40000");
			database = client.GetDatabase("test");
			Type type = typeof(T);
			collection = database.GetCollection<T>(type.Name.ToLower());
		}

		public void DropDatabase()
		{
			client.DropDatabase("test");
		}
		public void InsertOne(T model)
		{
			collection.InsertOne(model);
		}
		public void InsertMany(params T[] modes)
		{
			collection.InsertMany(modes);
		}
		public IMongoQueryable<T> Select()
		{
			return collection.AsQueryable<T>();
		}
		public IMongoQueryable<T> Select(int pageIndex, int pageSize)
		{
			return collection.AsQueryable<T>().Skip(pageSize * (pageIndex - 1)).Take(pageSize);
		}
		public IMongoQueryable<T> Select(Expression<Func<T, bool>> predicate, Expression<Func<T, object>> keySelector, int pageIndex, int pageSize)
		{
			return collection.AsQueryable<T>().Where(predicate).OrderBy(keySelector).Skip(pageSize * (pageIndex - 1)).Take(pageSize);
		}
		public void UpdateMany(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
		{
			collection.UpdateMany(filter, update);
		}

		public void UpdateOne(Expression<Func<T, bool>> filter, T update)
		{
			collection.ReplaceOne(filter, update);
		}

		public void DeleteMany(Expression<Func<T, bool>> filter)
		{
			collection.DeleteMany(filter);
		}

	}
}

