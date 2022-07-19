
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using Practice.Interface;
using Practice.Interface.AutofacExtension;

namespace Practice.Services
{ 
    public class CompanyService : ICompanyService
    {
        /// <summary>
        /// 这里是一个属性：如果支持属性注入---CompanyService构造完成以后，要吧这个属性的实例构建出来；
        /// </summary>
         [CustomProperty]
        public ISysUserService _ISysUserService { get; set; }

        public ISysUserService _ISysUserServiceNoAttribute { get; set; }
         
        public ISysUserService _ISysUserServiceCtor = null;
        public CompanyService(ISysUserService iSysUserService)
        {
            this._ISysUserServiceCtor = iSysUserService;
        }

        //CompanyService构造出来以后，如果_ISysUserServiceMethod有值 方法注入就成功
        public ISysUserService _ISysUserServiceMethod = null;
          
        public void SetService(ISysUserService iSysUserService)
        {
            this._ISysUserServiceMethod = iSysUserService;
        }


        public int GetId(int id)
        {
            return id;
        }
    }
}
