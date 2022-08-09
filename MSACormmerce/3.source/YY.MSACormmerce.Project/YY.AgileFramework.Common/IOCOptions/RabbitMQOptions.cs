using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.AgileFramework.Common.IOCOptions
{
    public class RabbitMQOptions
    {
        ///// <summary>
        ///// exchange---queue
        ///// </summary>
        //private static Dictionary<string, string> RabbitMQ_Mapping = new Dictionary<string, string>();
        //private static readonly object RabbitMQOptions_Lock = new object();
        //public void Init(string exchangeName, string queueName)
        //{
        //    lock (RabbitMQOptions_Lock)
        //    {
        //        RabbitMQ_Mapping[exchangeName] = queueName;
        //    }
        //}

        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class RabbitMQConsumerModel
    {
        /// <summary>
        /// 生产者指定，交换机
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 自己起的名字
        /// </summary>
        public string QueueName { get; set; }
    }

    public class RabbitMQExchangeQueueName
    {
        public static readonly string SKUCQRS_Exchange = "YY.MSACormmerce.SKUCQRS.Exchange";
        public static readonly string SKUCQRS_Queue_StaticPage = "YY.MSACormmerce.SKUCQRS.Queue.StaticPage";
        public static readonly string SKUCQRS_Queue_ESIndex = "YY.MSACormmerce.SKUCQRS.Queue.ESIndex";


        public static readonly string SKUWarmup_Exchange = "YY.MSACormmerce.Warmup.Exchange";
        public static readonly string SKUWarmup_Queue_StaticPage = "YY.MSACormmerce.Warmup.Queue.StaticPage";
        public static readonly string SKUWarmup_Queue_ESIndex = "YY.MSACormmerce.Warmup.Queue.ESIndex";

        /// <summary>
        /// 订单创建后的交换机
        /// </summary>
        public static readonly string OrderCreate_Exchange = "YY.MSACormmerce.OrderCreate.Exchange";
        public static readonly string OrderCreate_Queue_CleanCart = "YY.MSACormmerce.OrderCreate.Queue.CleanCart";

        /// <summary>
        /// 订单创建后的交换机,支付状态的
        /// </summary>
        public static readonly string OrderPay_Exchange = "YY.MSACormmerce.OrderPay.Exchange";
        public static readonly string OrderPay_Queue_RefreshPay = "YY.MSACormmerce.OrderPay.Queue.RefreshPay";

        /// <summary>
        /// 创建订单后的延时队列配置
        /// </summary>
        public static readonly string OrderCreate_Delay_Exchange = "YY.MSACormmerce.OrderCreate.DelayExchange";
        public static readonly string OrderCreate_Delay_Queue_CancelOrder = "YY.MSACormmerce.OrderCreate.DelayQueue.CancelOrder";

        /// <summary>
        /// 秒杀异步的
        /// </summary>
        public static readonly string Seckill_Exchange = "YY.MSACormmerce.Seckill.Exchange";
        public static readonly string Seckill_Order_Queue = "YY.MSACormmerce.Seckill.Order.Queue";


        /// <summary>
        /// CAP队列名称
        /// </summary>
        public const string Order_Stock_Decrease = "RabbitMQ.MySQL.Order-Stock.Decrease";
        public const string Order_Stock_Resume = "RabbitMQ.MySQL.Order-Stock.Resume";
        public const string Stock_Logistics = "RabbitMQ.MySQL.Stock-Logistics";

        public const string Pay_Order_UpdateStatus = "RabbitMQ.MySQL.Pay_Order.UpdateStatus";
    }

}
