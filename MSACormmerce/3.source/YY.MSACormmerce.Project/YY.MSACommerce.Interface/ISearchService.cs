using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YY.MSACommerce.Model;
using YY.MSACommerce.Model.Search;

namespace YY.MSACommerce.Interface
{
	public interface ISearchService
	{
		SearchResult<Goods> search(SearchRequest searchRequest);
		public void ImpDataBySpu();
		public SearchResult<Goods> GetData(SearchRequest  searchRequest);
		public Goods GetGoodsBySpuId(long spuId);
	}
}
