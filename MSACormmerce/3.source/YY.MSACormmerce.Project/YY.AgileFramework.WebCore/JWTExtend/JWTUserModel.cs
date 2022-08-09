using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YY.AgileFramework.WebCore.JWTExtend
{
    public class JWTUserModel
    {
        public int id { get; set; }
        public string username { get; set; }
    }
}