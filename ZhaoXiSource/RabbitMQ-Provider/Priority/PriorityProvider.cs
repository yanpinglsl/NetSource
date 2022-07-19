using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.Priority
{
    public class PriorityProvider
    {
        public static void SendMessage()
        {
            string exchange = "pro.exchange";
            string queueName = "pro.queue";
            const string MessagePrefix = "message_";
            const int PublishMessageCount = 10;
            byte messagePriority = 0;

            using (var connection = RabbitMQHelper.GetConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange, type: ExchangeType.Fanout, durable: true, autoDelete: false);

                    //x-max-priority属性必须设置，否则消息优先级不生效
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object> { { "x-max-priority", 10 } });
                    channel.QueueBind(queueName, exchange, queueName);

                    //向该消息队列发送消息message
                    Random random = new ();
                    for (int i = 0; i < PublishMessageCount; i++)
                    {
                        var properties = channel.CreateBasicProperties();
                        messagePriority = (byte)random.Next(1, 10);
                        properties.Priority = messagePriority;//设置消息优先级，取值范围在1~9之间。
                        var message = MessagePrefix + i.ToString()+"____"+messagePriority;
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish(exchange: exchange, routingKey: ExchangeType.Fanout, basicProperties: properties, body: body);
                        Console.WriteLine($"{DateTime.Now.ToString()} Send {message} , Priority {messagePriority}");
                    }
                }
            }
        }
    }
}
