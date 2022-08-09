﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YY.MSACommerce.DTOModel.DTO
{
    /// <summary>
    /// DTOJWTUser   给JWT使用的
    /// </summary>
    public class DTOJWTUser
    {
        public int id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string phone { get; set; }

        public DateTime created { get; set; }

        public string salt { get; set; }
    }
}
