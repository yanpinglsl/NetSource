using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Noraml
{
    public class Receive
    {
        public static void ReceiveMessage()
        {
            // 消费者消费是队列中消息
            string queueName = "normal";
            var connection = RabbitMQHelper.GetConnection("192.168.200.104", 5672);
            {
                var channel = connection.CreateModel();
                {
                    // 当初次启动消息队列时，所订阅的消息队列还不存在，
                    //这时如果你先启动是消费端就会异常
                    //为了避免此类情况的发送，消费端与生产端都要写交换机、队列绑定的处理
                    channel.QueueDeclare(queueName, false, false, false, null);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received +=(model, ea) => {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        Console.WriteLine(" Normal Received => {0}", message);
                    }; 
                    channel.BasicConsume(queueName,true, consumer);
                }
            }
          
        } 
    }
}
