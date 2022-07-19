using Microsoft.Extensions.Configuration;
using ServiceStack.Redis;
using System;
using System.Threading.Tasks;

namespace LockService
{
    class Program
    {
        static void Main(string[] args)
        {
            //命令行参数启动  秒杀，到一个时刻的时候，开始秒杀
            //dotnet Zhaoxi.LockService.dll --minute=21
            var builder = new ConfigurationBuilder().AddCommandLine(args);
            var configuration = builder.Build();
            int minute = int.Parse(configuration["minute"]);
            using (var client = new RedisClient("127.0.0.1", 6379))
            {

                //先把库存数量预支进去
                client.Set<int>("inventoryNum", 20);
                // // 订单  如果订单是10 或者<=10 说明没有问题
                client.Set<int>("orderNum", 0);

            }
            Console.WriteLine("ok");
            Console.WriteLine($"在{minute}分0秒正式开启秒杀！");
            var flag = true;
            while (flag)
            {

                if (DateTime.Now.Minute == minute)
                {
                    flag = false;

                    #region 同步执行
                    //启动单个服务可以正常运行，启动多个服务会出现超卖的情况
                    //for (int  i = 0; i < 30; i++)
                    //{
                    //    NormalSecondsKill.Show();
                    //}
                    #endregion

                    #region 异步执行
                    //加锁后启动单个服务可以正常运行，启动多个服务会出现超卖的情况
                    Parallel.For(0, 30, (i) =>
                    {
                        int temp = i;
                        Task.Run(() =>
                        {
                            //lock锁
                            //NormalSecondsKill.Show();
                            // 阻塞锁   拿不到锁的时候，还有等待，等待时间都交给业务程序（使用率40%）
                            //BlockingLock.Show(i, "akey", TimeSpan.FromSeconds(100));

                            //非阻塞锁 
                            //把这个补偿重试，能不能交给用户，，， 浏览器，， 你想要重试，就重试，不想要，（使用率60%）
                            //如果拿到锁，就返回true，如果拿不到，就false---
                            //ImmediatelyLock.Show(i, "akeye", TimeSpan.FromSeconds(100));

                            //红锁
                            //（1）解决单点故障
                            //（2）与阻塞锁相比，可以防止删除别人的锁
                            RadLockSkill.Show(i, "akey", TimeSpan.FromSeconds(100));

                        });
                    });
                    #endregion
                    //Parallel.For(0, 30, (i) =>
                    //{
                    //    int temp = i;
                    //    Task.Run(() =>
                    //    {

                    //        //NormalSecondsKill.Show();
                    //        #region MyRegion
                    //        // 阻塞锁   拿不到锁的时候，还有等待，等待时间都交给业务程序
                    //        //BlockingLock.Show(i, "akey", TimeSpan.FromSeconds(100));
                    //        // 把这个补偿重试，能不能交给用户，，， 浏览器，， 你想要重试，就重试，不想要，
                    //        // 如果拿到锁，就返回true，如果拿不到，就false---
                    //        //// 非阻塞锁 


                    //        //ImmediatelyLock.Show(i, "akey", TimeSpan.FromSeconds(100));

                    //        ////
                    //        RadLockSkill.Show(i, "akey", TimeSpan.FromSeconds(100));

                    //        ////});
                    //        //Thread.Sleep(500); 
                    //        #endregion

                    //    });


                    //});

                    Console.ReadKey();
                }
            }
        }
    }
}
