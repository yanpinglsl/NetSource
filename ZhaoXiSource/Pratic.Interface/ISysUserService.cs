 
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
 

namespace Practice.Interface
{
    public interface ISysUserService//:IBaseService 
    {
        public int GetId(int id);
    }
}
