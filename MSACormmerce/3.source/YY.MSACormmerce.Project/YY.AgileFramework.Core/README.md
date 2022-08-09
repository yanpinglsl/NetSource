## 1 准备ES环境

```
ES配置启动
docker run -d --name elasticsearch -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" -e ES_JAVA_OPTS="-Xms100m -Xmx200m" elasticsearch:7.2.0
```

## 2  准备Kafka环境

docker-compose.yml

```
# KAFKA_ZOOKEEPER_CONNECT: "zoo1:2181" 必须是2181内部端口


version: '2'

services:
  zoo1:
    image: wurstmeister/zookeeper
    restart: unless-stopped
    hostname: zoo1
    ports:
      - "6181:2181"
    container_name: zookeeper_kafka

  # kafka version: 1.1.0
  # scala version: 2.12
  kafka1:
    image: wurstmeister/kafka
    ports:
      - "9092:9092"
    environment:
      KAFKA_ADVERTISED_HOST_NAME: 192.168.3.230
      KAFKA_ZOOKEEPER_CONNECT: "zoo1:2181"
      KAFKA_BROKER_ID: 1
      KAFKA_OFFSETS_TOPIC_REPLICATION_FACTOR: 1
      KAFKA_CREATE_TOPICS: "stream-in:1:1,stream-out:1:1"
    depends_on:
      - zoo1
    container_name: kafka
```

kafka配置

```
 "KafkaOptions": {
    "BrokerList": "192.168.3.230:9092",
    "TopicName": "kafkalog"
  }
```

KafkaTool连接：

192.168.3.230

6181端口



## 3  准备ELK


```
ES配置启动
docker run -d --name elasticsearch -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" -e ES_JAVA_OPTS="-Xms100m -Xmx200m" elasticsearch:7.2.0

启动logstash
docker run --rm -it --privileged=true -p 9600:9600  -d -v /eleven/logstash/kafkalog.conf:/usr/share/logstash/pipeline/logstash.conf -v /eleven/logstash/log/:/home/public/  -v /eleven/logstash/logstash.yml:/usr/share/logstash/config/logstash.yml logstash:7.2.0

Kibana配置(ES配置)
docker run -p 5601:5601 -d -e ELASTICSEARCH_URL=http://192.168.3.230:9200   -e ELASTICSEARCH_HOSTS=http://192.168.3.230:9200 kibana:7.2.0  
```


```
Kibana,需要根据ES里面不同的index去配置模板
选择ES的Index名称，可以将多个ES的Index库合并到一起，完成跨库查询
也就是LogStash配置的ES的Index名称
```

http://192.168.3.230:5601/



## 4 .NET 项目对接log4net写入数据到kafka

~~~
1 nuget添加引用
using Microsoft.Extensions.Logging.Log4Net.AspNetCore
using log4net.Kafka.Core

2  Program-->添加引用
.ConfigureLogging(loggingBuilder =>
			{
				loggingBuilder.AddFilter("System", LogLevel.Warning);
				loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
				loggingBuilder.AddLog4Net();
			})

~~~
3  当前项目新增 log4net.config

~~~
<?xml version="1.0" encoding="utf-8" ?>
<log4net>
  <appender name="KafkaAppender" type="log4net.Kafka.Core.KafkaAppender, log4net.Kafka.Core">
    <KafkaSettings>
      <broker value="192.168.3.230:9092" />
      <topic value="kafkalog" />
    </KafkaSettings>
    <layout type="log4net.Kafka.Core.KafkaLogLayout,log4net.Kafka.Core" >
      <appid value="YY.MSACommerce.BrandMicroservice" />
    </layout>
  </appender>
  <root>
    <level value="ALL"/>
    <appender-ref ref="KafkaAppender" />
  </root>
</log4net>
~~~