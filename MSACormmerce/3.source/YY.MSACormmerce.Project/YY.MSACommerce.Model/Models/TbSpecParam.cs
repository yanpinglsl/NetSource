using System;
using System.Collections.Generic;

namespace YY.MSACommerce.Model
{
    public partial class TbSpecParam
    {
        public long Id { get; set; }
        public long Cid { get; set; }
        public long? GroupId { get; set; }
        public string Name { get; set; }
        public bool Numeric { get; set; }
        public string Unit { get; set; }
        public bool? Generic { get; set; }
        public bool? Searching { get; set; }
        public string Segments { get; set; }
    }
}
