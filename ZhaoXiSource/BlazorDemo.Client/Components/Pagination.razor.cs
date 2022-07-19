using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Components
{
    public  partial class Pagination
    {
		[Parameter]
		public MetaData Metadata { get; set; }

		[Parameter]
		public int Spread { get; set; }

		// 这是一个回调方法 // 就是一个事件 v// 委托 // 委托是可以把一个方法当做参数去传递
		[Parameter]
		public EventCallback<int> SelectedPage { get; set; }

		private List<PagingLink> _links;

		protected override void OnParametersSet()
		{
			CreatePaginationLinks();
		}

		private void CreatePaginationLinks()
		{
			_links = new List<PagingLink> { new PagingLink(Metadata.CurrentPage - 1, Metadata.HasPrevious, "上一页") };
			for (int i = 1; i <= Metadata.TotalPages; i++)
			{
				if (i >= Metadata.CurrentPage - Spread && i <= Metadata.CurrentPage + Spread)
				{
					_links.Add(new PagingLink(i, true, i.ToString()) { Active = Metadata.CurrentPage == i });
				}
			}
			_links.Add(new PagingLink(Metadata.CurrentPage + 1, Metadata.HasNext, "下一页"));
		}

		private async Task OnSelectedPage(PagingLink link)
		{
			if (link.Page == Metadata.CurrentPage || !link.Enabled)
				return;
			Metadata.CurrentPage = link.Page;
			// 调用了这个事件，如果这个事件不为null ,则会执行，调用具体方法
			await SelectedPage.InvokeAsync(link.Page);
		}

	}
}
