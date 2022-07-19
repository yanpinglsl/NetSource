using BlazorDemo.Client.Service;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Pages
{
	public partial class UserinfoPage
	{

		[Inject]
		public IUserHttpRepository userHttpRepository { get; set; }

		public List<Userinfo> page_Userinfo = new List<Userinfo>();
		// 这是元数据，，分页详情,分页组件需要的
		public MetaData Metadata { get; set; } = new MetaData();
		// 默认分页相关的信息
		private readonly UserParameters userParameters = new UserParameters();

		// 分页组件的回调方法 ，是不是传递当前的页码
		public async Task SelectedPage(int page)
		{
			// 带着页码去查询后台的数据
			userParameters.PageNumber = page;
			await GetUsers();
		}
		[Inject]
		public IUserHttpRepository UserHttpRepository { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await GetUsers();
		}

		public async Task GetUsers()
		{
			var pagingResponse = await UserHttpRepository.GetUserinfos(userParameters);
			page_Userinfo = pagingResponse.Items;
			Metadata = pagingResponse.MetaData;
		}
		// 删除用户
		public async Task DeleteUser(int id)
		{
			await UserHttpRepository.DeleteUser(id.ToString());
			await GetUsers();
		}
		// 不想调用自带js方法，想调用，自定义js方法
		[Inject]
		public IJSRuntime Js { get; set; }
		public string Result { get; set; }
		// 利用。net 高性能，编写和运行前端的代码
		public async Task SayHello()
		{
			// 调用自定义js方法
			Result = await Js.InvokeAsync<string>("sayHello", "Mike");
		}
		// 搜索回调的方法
		public async Task SearchChanged(string searchTerm)
		{
			Console.WriteLine(searchTerm);
			userParameters.PageNumber = 1;
			userParameters.SearchTerm = searchTerm;
			await GetUsers();
		}

		// 什么时候需要组件--自定义控件--实现重复利用， 就是有些页面部分会被经常使用，则可以封装成组件
	}
}
