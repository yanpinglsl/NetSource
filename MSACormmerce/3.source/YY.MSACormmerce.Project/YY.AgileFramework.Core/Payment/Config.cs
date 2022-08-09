using System;
using System.Collections.Generic;
using System.Web;

namespace YY.MSACommerce.Core.Payment
{
    /**
    * 	配置账号信息
    */
    public class Config
    {

        private static volatile IConfig config;
        private static object syncRoot = new object();

        public static IConfig GetConfig(){
            if(config==null){
                lock(syncRoot){
                    if (config == null)
                        config = new WxPayConfig();
                }
            }
            return config;
        } 
    }
}