using RabbitMQ.Client;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Provider.DXL
{
    public class DLXSend
    {

        public static void SendMessage()
        {
            var exchangeA = "exchange";
            var routeA = "routekey";
            var queueA = "queue";

            var exchangeD = "dlx.exchange";
            var routeD = "dlx.route";
            var queueD = "dlx.queue";

            using (var connection = RabbitMQHelper.GetConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    // 创建死信交换机
                    channel.ExchangeDeclare(exchangeD, type: ExchangeType.Fanout, durable: true, autoDelete: false);
                    // 创建死信队列
                    channel.QueueDeclare(queueD, durable: true, exclusive: false, autoDelete: false);
                    // 绑定死信交换机和队列
                    channel.QueueBind(queueD, exchangeD, routeD);

                    channel.ExchangeDeclare(exchangeA, type: ExchangeType.Fanout, durable: true, autoDelete: false);
                    channel.QueueDeclare(queueA, durable: true, exclusive: false, autoDelete: false, arguments: 
                                        new Dictionary<string, object> {
                                             { "x-dead-letter-exchange",exchangeD}, //设置当前队列的DLX
                                             { "x-dead-letter-routing-key",routeD}, //设置DLX的路由key，DLX会根据该值去找到死信消息存放的队列
                                             { "x-message-ttl",10000} //设置消息的存活时间，即过期时间
                                         });
                    channel.QueueBind(queueA, exchangeA, routeA);


                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;
                    //发布消息
                    channel.BasicPublish(exchange: exchangeA,
                                         routingKey: routeA,
                                         basicProperties: properties,
                                         body: Encoding.UTF8.GetBytes("hello rabbitmq message"));
                }
            }
            
        } 
    }
}
