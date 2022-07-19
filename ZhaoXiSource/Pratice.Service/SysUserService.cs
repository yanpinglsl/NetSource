using System;
using System.Collections.Generic;
using System.Text;
using Practice.Interface;

namespace Practice.Services
{
    public class SysUserService : ISysUserService
    {
        public SysUserService()
        {
            Console.WriteLine($"{this.GetType().Name}被构造。。。。");
        }

        public int GetId(int id)
        {
            throw new NotImplementedException();
        }
    }
}
