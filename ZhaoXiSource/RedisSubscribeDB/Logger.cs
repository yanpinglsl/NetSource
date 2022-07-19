using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace RedisSubscribeDB
{
	class Logger
	{


 //  开发中 记录日志不用 关系型数据库，用es，mogngodb
		public static void WriteLogByDB(string content)
		{
			string ConStr = "server=.;database=demo;uid=sa;pwd=123456";
			using (SqlConnection sqlcon = new SqlConnection(ConStr))
			{
				sqlcon.Open();
				var sql = "insert into log(content) values(@content)";
				SqlCommand sqlCommand = new SqlCommand(sql, sqlcon);
				sqlCommand.Parameters.Add(new SqlParameter() { ParameterName="@content",Value=content });
				sqlCommand.ExecuteNonQuery();
			}
		}
	}
}
