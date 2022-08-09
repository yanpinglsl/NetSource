using Consul;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.AgileFramework.Core.ConsulExtend.DistributedExtend
{
    public class ConsulDistributed : IConsulDistributed
    {
        #region Identity
        protected ConsulClientOptions _ConsulClientOption = null;
        private static string prefix = "consullock_";  // 同步锁参数前缀
        public ConsulDistributed(IOptionsMonitor<ConsulClientOptions> options)
        {
            this._ConsulClientOption = options.CurrentValue;
        }

        #endregion
        public async Task KVShow()
        {
            using (ConsulClient client = new ConsulClient(c =>
            {
                c.Address = new Uri($"http://{this._ConsulClientOption.IP}:{this._ConsulClientOption.Port}/");
                c.Datacenter = this._ConsulClientOption.Datacenter;
            }))
            {
                await client.KV.Put(new KVPair("Eleven") { Value = Encoding.UTF8.GetBytes("This is Teacher") });

                System.Diagnostics.Debug.WriteLine($"刚写入,client.KV.Get(\"Eleven\")={this.ByteArrayToString(client.KV.Get("Eleven").Result.Response.Value)}");

                await client.KV.Put(new KVPair("Eleven") { Value = Encoding.UTF8.GetBytes("This is Teacher2222") });
                System.Diagnostics.Debug.WriteLine($"更新后,client.KV.Get(\"Eleven\")={this.ByteArrayToString(client.KV.Get("Eleven").Result.Response.Value)}");

                System.Diagnostics.Debug.WriteLine("This is client.KV.List(\"Ele\").Result.Response.ToList()");
                foreach (var kVPair in client.KV.List("Ele").Result.Response.ToList())
                {
                    System.Diagnostics.Debug.WriteLine($"{kVPair.Key} :{kVPair.Value}");
                }

                await client.KV.Delete("Eleven");
                System.Diagnostics.Debug.WriteLine($"删除后,client.KV.Get(\"Eleven\") is null? {client.KV.Get("Eleven").Result.Response is null}");
            }
        }

        private string ByteArrayToString(byte[] byteArray)
        {
            return Encoding.UTF8.GetString(byteArray);
        }

        #region 分布式锁

        private ConsulClient consulClient;
        private static readonly object ConsulClient_Lock = new object();
        private void InitConsulClient()
        {
            if (consulClient == null)
            {
                lock (ConsulClient_Lock)
                {
                    if (consulClient == null)
                        consulClient = new ConsulClient(c =>
                        {
                            c.Address = new Uri($"http://{this._ConsulClientOption.IP}:{this._ConsulClientOption.Port}/");
                            c.Datacenter = this._ConsulClientOption.Datacenter;
                        });
                }
            }
        }

        /// <summary>
        /// 需要先初始化
        /// </summary>
        /// <param name="key"></param>
        public Task<IDistributedLock> AcquireLock(string key)
        {
            this.InitConsulClient();
            LockOptions opts = new LockOptions($"{prefix}{key}");//默认值
            //{
            //    LockRetryTime = TimeSpan.FromSeconds(5),
            //    LockWaitTime = TimeSpan.FromSeconds(3),
            //    MonitorRetryTime = TimeSpan.FromSeconds(1)
            //};
            return this.consulClient.AcquireLock(opts);
        }

        /// <summary>
        /// 包装了一层，委托嵌套
        /// </summary>
        /// <param name="key"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Task ExecuteLocked(string key, Action action)
        {
            this.InitConsulClient();
            //Console.WriteLine($"{prefix}{key}");
            LockOptions opts = new LockOptions($"{prefix}{key}");//默认值
            //{
            //    LockRetryTime = TimeSpan.FromSeconds(5),
            //    LockWaitTime = TimeSpan.FromSeconds(3),
            //    MonitorRetryTime = TimeSpan.FromSeconds(1)
            //};
            return this.consulClient.ExecuteLocked(opts, action);
        }

        public void Dispose()
        {
            if (this.consulClient != null)
            {
                this.consulClient.Dispose();
                this.consulClient = null;
            }
        }
        #endregion
    }
}
