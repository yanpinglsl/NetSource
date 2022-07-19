using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Kafka.Produce
{

    class ConfulentKafka
    {

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="event"></param>
        public static async Task Produce(string brokerlist, string topicname, string content)
        {
            string brokerList = brokerlist;
            string topicName = topicname;
            var config = new ProducerConfig
            {
                BootstrapServers = brokerList,
                // ack ---注意注意注意。。
                // 代码封装的原因，消费端也可以写，实际没有用到===
                // 数据写入保存机制--- 保证数据不丢失，而且会影响到我们性能，
                // 越高级别数据不丢失，则写入的性能越差
                Acks = Acks.All,
                // 幂等性，保证不会丢数据。。 
                EnableIdempotence = true,

                //LingerMs = 10000, //信息发送完，多久吧数据发送到broker里面去
                BatchNumMessages = 1,//控制条数，当内存的数据条数达到了，立刻马上发送过去
                                     // 只要上面的两个要求符合一个，则后台线程立刻马上把数据推送给broker
                                     // 可以看到发送的偏移量，如果没有偏移量，则就是没有写成功
                MessageSendMaxRetries = 3,//补偿重试，发送失败了则重试 
                                          //  Partitioner = Partitioner.Random

            };

            using (var producer = new ProducerBuilder<string, string>(config)
                //.SetValueSerializer(new CustomStringSerializer<string>())
                //	.SetStatisticsHandler((o, json) =>
                //{
                //	Console.WriteLine("json");
                //	Console.WriteLine(json);
                //})
                .Build())
            {

                Console.WriteLine("\n-----------------------------------------------------------------------");
                Console.WriteLine($"Producer {producer.Name} producing on topic {topicName}.");
                Console.WriteLine("-----------------------------------------------------------------------");
                try
                {
                    // Key 注意是做负载均衡，注意： 比如，有三个节点，一个topic，创建了三个分区。。一个节点一个分区，但是，如果你在写入的数据的时候，没有写key,这样会导致，所有的数据存放到一个分区上面。。。
                    // ps：如果用了分区，打死也要写key .根据自己的业务，可以提前配置好，
                    // key的随机数，可以根据业务，搞一个权重，如果节点的资源不一样，合理利用资源，可以去写一个
                    //var deliveryReport = await producer.
                    //ProduceAsync(
                    //topicName, new Message<string, string> { Key = (new Random().Next(1, 10)).ToString(), Value = content });
                    //Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                    //除非您的应用程序是高度并发的，否则最好避免像上面这样的同步执行
                    //因为它会极大地降低最大吞吐量
                    //或者您可以使用该Produce方法，该方法将传递处理程序委托作为参数

                    //首先，消息传递（或失败）的通知严格按照代理确认的顺序进行。使用 ProduceAsync，情况并非如此，因为Tasks可以在任何线程池线程上完成
                    //其次，Produce性能更高，因为Task基于更高级别的 API存在不可避免的开销。
                    producer.Produce(topicName, new Message<string, string> { Key = (new Random().Next(1, 10)).ToString(), Value = content },handler);
                    producer.Flush(TimeSpan.FromSeconds(1000));
                }
                catch (ProduceException<string, string> e)
                {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }
        public static void handler(DeliveryReport<string, string> delivery)
        {
            Console.WriteLine($"delivered to: {delivery.TopicPartitionOffset}");
        }

    }
}