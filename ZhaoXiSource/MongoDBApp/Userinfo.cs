using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Zhaoxi.MongodbApp
{
	public class Userinfo
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Address { get; set; }
		public string Sex { get; set; }

		public string dsfsdfsd { get; set; }

		public int Age { get; set; }
		public DetpInfo DetpInfo { get; set; }
	}
	 
	public class DetpInfo
	{
		public int DeptId { get; set; }
		public string DeptName { get; set; }
	}
}
