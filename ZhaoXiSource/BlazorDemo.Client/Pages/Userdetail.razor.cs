using BlazorDemo.Client.Service;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Pages
{
    public partial class Userdetail
	{
		[Parameter]
		public int Userid { get; set; }

		public Userinfo Userinfo = new Userinfo();

		[Inject]
		public IUserHttpRepository UserHttpRepository { get; set; }
		protected override async Task OnInitializedAsync()
		{
			Userinfo = await UserHttpRepository.GetUserinfosById(Userid);

		}

		public void Button_Click()
		{
			Userinfo.UserName = "clay";
		}
	}
}
