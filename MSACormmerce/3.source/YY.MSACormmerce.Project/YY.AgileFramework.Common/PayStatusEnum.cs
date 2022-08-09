using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YY.AgileFramework.Common
{
	public enum PayStatusEnum
	{

		// NOT_PAY(0), SUCCESS(1), FAIL(2);
		NOT_PAY = 0, // 未支付
		SUCCESS = 1, // 支付成功
		FAIL = 2     // 支付失败
	}
}
