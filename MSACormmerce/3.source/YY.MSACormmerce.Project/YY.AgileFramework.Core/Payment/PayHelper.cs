using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.AgileFramework.Common;

namespace YY.MSACommerce.Core.Payment
{
    /// <summary>
    /// 支付工具类
    /// </summary>
    public class PayHelper
    {
        public static readonly string KEY_PAY_PREFIX = "order:pay:url:";
        private CacheClientDB _cacheClientDB;
        private HttpContext _httpContext;

        public PayHelper(CacheClientDB cacheClient)
        {
            _cacheClientDB = cacheClient;
        }

        /// <summary>
        /// 创建支付连接
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="description"></param>
        /// <param name="totalPay"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public string CreatePayUrl(long orderId, string description, long totalPay, HttpContext httpContext)
        {
            this._httpContext = httpContext;
            // 定义返回的支付连接
            string url;
            //从缓存中取出支付连接
            string key = KEY_PAY_PREFIX + orderId;
            try
            {
                url = _cacheClientDB.Get<string>(key);
                if (null != url)
                {
                    return url;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"查询缓存付款链接异常，订单号：{orderId}", e);
            }
            try
            {
                // 构建支付需要的参数对象
                WxPayData data = new WxPayData();
                //描述
                data.SetValue("body", description);
                //订单号
                data.SetValue("out_trade_no", orderId.ToString());
                data.SetValue("product_id", orderId.ToString());
                //货币（默认就是人民币）
                data.SetValue("fee_type", "CNY");
                //TODO 总金额 （模拟1分钱， 线上环境换成真实价格）
                data.SetValue("total_fee", /*totalPay.ToString()*/ 1);
                //调用微信支付的终端ip
                data.SetValue("spbill_create_ip", "0.0.0.0");
                //回调地址
                data.SetValue("notify_url", Config.GetConfig().GetNotifyUrl());
                //交易类型为扫码支付
                data.SetValue("trade_type", "NATIVE");

                WxPayData result = WxPayApi.UnifiedOrder(data, httpContext);//调用统一下单接口
                url = result.GetValue("code_url").ToString();//获得统一下单接口返回的二维码链接

                //将连接缓存到Redis中，失效时间2小时
                _cacheClientDB.Set(key, url, TimeSpan.FromHours(2));
            }
            catch (Exception e)
            {
                throw new Exception("生成支付链接连接失败", e);
            }

            return url;
        }


        /// <summary>
        /// （调用微信API）根据订单ID查询订单信息,全部信息
        /// </summary>
        /// <param name="transaction_id"></param>
        /// <returns></returns>
        public WxPayData QueryOrderById(long orderId)
        {
            WxPayData req = new WxPayData();
            req.SetValue("out_trade_no", orderId.ToString());
            WxPayData res = WxPayApi.OrderQuery(req, _httpContext);
            return res;// 返回查询数据
        }
    }
}
