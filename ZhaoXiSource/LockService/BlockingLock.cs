using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ServiceStack.Redis;
namespace LockService
{
	public class BlockingLock
	{


		//这个是阻塞的锁
		public static void Show(int i, string key, TimeSpan timeout)
		{
			using (var client = new RedisClient("127.0.0.1", 6379))
			{
				// 这个就是一个阻塞锁的用法。。
				//timeout : 锁的占有时间，这个时间不是redis里面的过期时间，就是写进去一个值。
				//timeout : 如果拿不到锁，我需要自身轮询等待多久，，先去判断能不能拿到锁，如果拿不到，等多久，如果等多久之后，还拿不到，则直接不会执行里面的代码。。
				// timeout 问题。。 10s ,过期时间是10s-- 2021-06-05 20:27:50 2021-06-05 20:27:60
				// 第二个线程拿锁的时间是 2021-06-05 20:27:71
				using (var datalock = client.AcquireLock("DataLock:" + key, timeout))
				{
					// 首先判断这个DataLock的key不在，说明没有人拿到锁，我就可以写一个key:DataLock,values=timeout
					// 如果这个key存在，，判断时间过没过期，如果过期了，则我们可以拿到锁，如果没有，则进行根据你的等待时间在去轮询判断锁在不在
					//库存数量

					// 如果多个线程在判断锁的时候，发现锁没有过期，然后一直等待，，
					// 刚好三个线程，并发同一时间，发geiredis请求，判断这个key,此时此刻，刚好，这个key不在，会不会出现，三个人都去set key ，大不了把值覆盖，但是可能这个三个线程都拿到锁。。。 
					// AcquireLock 底层
					// 判断这个锁在不在时候，都会查询出当前的key，还有我们的版本号
					// 如果锁过期。然后在抢锁的过程--用事务的版本--- 带着版本号提交，如果版本号一致，则拿到锁，如果不一致，那锁失败 
					var inventory = client.Get<int>("inventoryNum");
					if (inventory > 0)
					{
						client.Set<int>("inventoryNum", inventory - 1);
						//订单数量+1
						var orderNum = client.Incr("orderNum");
						Console.WriteLine($"{i}抢购成功*****线程id：{ Thread.CurrentThread.ManagedThreadId.ToString("00")},库存：{inventory},订单数量：{orderNum}");
					}
					else
					{
						Console.WriteLine($"{i}抢购失败");

						Thread.Sleep(100); //20s

					}
					// todo 比如还有其他时间--  2021-06-05 20:27:61
					//  查询mysql数据库，数据量小的时候，2s 10s

					// 从redis 里面删除DataLock  回收锁 时间问题 -- 本身存在的问题。。。设计这个时间，已经经过大量测试，选择一个适合时间。。。 
					// timeout 一个就是value。  guid---唯一标识
					//timeout 拿不到锁轮询的时候
					//timeout就是操作redis的时候，过期时间，让redis后台去删除这个key..
					// 线程用完锁之后，带着value--如果唯一标识，你就可以删除，这个锁，不然就是别人的锁，你是删不掉的。。 radlock 就是这么设置的===

				}

			}
		}
	}
}