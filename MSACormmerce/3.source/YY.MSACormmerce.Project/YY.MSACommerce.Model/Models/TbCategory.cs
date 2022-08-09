using System;
using System.Collections.Generic;

namespace YY.MSACommerce.Model
{
    public partial class TbCategory
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long ParentId { get; set; }
        public bool IsParent { get; set; }
        public int Sort { get; set; }
    }
}
