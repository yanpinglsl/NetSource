using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Confirm
{
    public class Transaction
    {
        /// <summary>
        /// 使用事务方式确保数据正确到达消息服务端
        /// </summary>
        public static void TransactionMode()
        {
            string exchange = "tx-exchange";
            string queue = "tx-queue";
            string routeKey = "direct_key";
           
            using (IConnection conn = RabbitMQHelper.GetConnection())
            {
                using (var channel = conn.CreateModel())
                {
                    try
                    {
                        channel.TxSelect(); //用于将当前channel设置成transaction事务模式
                        channel.ExchangeDeclare(exchange, ExchangeType.Direct);
                        channel.QueueDeclare(queue, false, false, false, null);
                        channel.QueueBind(queue, exchange, routeKey, null);

                        var properties = channel.CreateBasicProperties();
                        // properties.Persistent = true;
                        // properties.DeliveryMode = 2;

                        Console.Write("输入发送的内容：");
                        var msg = Console.ReadLine();
                           
                        byte[] message = Encoding.UTF8.GetBytes("发送消息:" + msg);
                        channel.BasicPublish(exchange, routeKey, properties, message);
                        // 故意抛异常
                        throw new Exception("出现异常");

                        channel.TxCommit();//txCommit用于提交事务
                    }
                    catch (Exception ex)
                    {
                        if (channel.IsOpen)
                        {
                            Console.WriteLine("触发事务回滚");
                            channel.TxRollback();
                        }
                        
                    }
                }
            }
        }
    }
}
