using System;
using System.Collections.Generic;

namespace YY.MSACommerce.Model
{
    public partial class TbSeckillOrder
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long OrderId { get; set; }
        public long SkuId { get; set; }
    }
}
