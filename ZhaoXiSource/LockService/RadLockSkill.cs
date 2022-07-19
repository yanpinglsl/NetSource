
using RedLockSource;
using ServiceStack.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


namespace LockService
{
    public class RadLockSkill
    {
        //docker run -d  -p 6379:6379 --name myredis redis 
        //docker run -d  -p 6380:6379  --name myredis1 redis 
        //docker run -d  -p 6381:6379  --name myredis2  redis 
        //docker run -d  -p 6382:6379  --name myredis3 redis 
        public static void Show(int i, string key, TimeSpan timeout)
        {
            //要部署多个Redis服务器，且相互之间独立（非集群）。
            //根据大多数，如果大多数节点写成功，就认为自己拿到锁，是业务代码自己去写
            var dlm = new Redlock(
                    ConnectionMultiplexer.Connect("120.78.170.106:6380"),
                    ConnectionMultiplexer.Connect("120.78.170.106:6381"),
                    ConnectionMultiplexer.Connect("120.78.170.106:6382")
                      );

            Lock lockObject;
            // true ,拿到锁,false 拿不到  、、阻塞锁（内部还是补偿重试）
            var isLocked = dlm.Lock(
                    key,
                    timeout,
                    out lockObject
                     );


            if (isLocked)
            {
                try
                {
                    using (var client = new RedisClient("127.0.0.1", 6379))
                    {
                        //库存数量
                        var inventory = client.Get<int>("inventoryNum");
                        if (inventory > 0)
                        {
                            client.Set<int>("inventoryNum", inventory - 1);
                            //订单数量
                            var orderNum = client.Incr("orderNum");
                            Console.WriteLine($"{i}抢购成功*****线程id：{ Thread.CurrentThread.ManagedThreadId.ToString("00")},库存：{inventory},订单数量：{orderNum}");
                        }
                        else
                        {
                            Console.WriteLine($"{i}抢购失败:原因，没有库存");
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    // 删锁 
                    dlm.Unlock(lockObject);
                }
            }
            else
            {
                Console.WriteLine($"{i}抢购失败：原因：没有拿到锁");
            }
        }

    }
}
