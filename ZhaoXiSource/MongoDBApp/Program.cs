using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Zhaoxi.MongodbApp
{
	class Program
	{
		static void Main(string[] args)
		{

		 
			Console.WriteLine("");
			//把当前的数据库中的所有数据清理掉了
			//LinqOperation.DropDatabase();
			try
			{
				#region json 操作

				{
					Console.WriteLine("***********json********");
					JsonOperation.Show();
				}
				#endregion

				{
					//Console.WriteLine("***********Linq********");
					//LinqOperation.InserOne();
				}
				{
			//	Transaction.Show();
				}

			}
			catch (Exception ex)
			{

				Console.WriteLine(ex.Message);
			}
		}
	}
}
