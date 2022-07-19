﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ_Consumer.Priority
{
    public class PriorityConsumer
    {
        public static void ConsumerMessage()
        {
            string exchange = "pro.exchange";
            string queueName = "pro.queue";
            using (var connection = RabbitMQHelper.GetConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    Console.WriteLine("连接成功！");
                    channel.ExchangeDeclare(exchange, ExchangeType.Fanout, durable: true, autoDelete: false);
                    channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: new Dictionary<string, object> { { "x-max-priority", 10 } });
                    channel.QueueBind(queueName, exchange, queueName);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        byte[] body = ea.Body.ToArray();
                        string message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(message);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };

                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                    Console.ReadLine();
                }
            }
        }
    }
}
