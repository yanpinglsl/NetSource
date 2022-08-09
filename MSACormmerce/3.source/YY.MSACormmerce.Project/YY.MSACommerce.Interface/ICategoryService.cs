using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.MSACommerce.Model;

namespace YY.MSACommerce.Interface
{
	public interface ICategoryService
	{

		List<TbCategory> QueryCategoryByPid(long pid);

		List<TbCategory> QueryCategoryByIds(List<long> ids);

		List<TbCategory> QueryAllByCid3(long id);
	}
}
