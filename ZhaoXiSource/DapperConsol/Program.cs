using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperConsol
{
    class Program
    {
        public static string Conn = @"Data Source=172.17.6.196\SQLEXPRESS;Initial Catalog=OrderService;User ID=sa;Pwd=1528938@Yan;Integrated Security=True;MultipleActiveResultSets=True";

        static void Main(string[] args)
        {
            QueryTest();
            Console.Read();
        }
        private static void InsertTest()
        {

            IDbConnection connection = new SqlConnection(Conn);

            var result = connection.Execute("Insert into Users values (@UserName, @Email, @Address)",
                                   new { UserName = "jack", Email = "380234234@qq.com", Address = "上海" });
        }
        private static void InsertBulkTest()
        {
            IDbConnection connection = new SqlConnection(Conn);

            //var result = connection.Execute("Insert into Users values (@UserName, @Email, @Address)",
            //                       new { UserName = "jack", Email = "380234234@qq.com", Address = "上海" });

            var usersList = Enumerable.Range(0, 10).Select(i => new Users()
            {
                Email = i + "qq.com",
                Address = "安徽",
                UserName = i + "jack"
            });

            var result = connection.Execute("Insert into Users values (@UserName, @Email, @Address)", usersList);
        }
        private static void QueryTest()
        {
            IDbConnection connection = new SqlConnection(Conn);

            var queryList = connection.Query<Users>("select * from Users where UserName=@UserName", new { UserName = "jack" });
        }
    }
    public class Users
    {
        public string Email { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
    }

}
