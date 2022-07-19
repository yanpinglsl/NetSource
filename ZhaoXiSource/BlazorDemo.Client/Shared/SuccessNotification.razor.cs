using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Shared
{
	public partial class SuccessNotification
	{
		private string _modalDisplay;
		private string _modalClass;
		private bool _showBackdrop;

		[Inject]
		public NavigationManager Navigation { get; set; }

		[Parameter]
		public string Url { get; set; }
		public void Show()
		{
			_modalDisplay = "block;";
			_modalClass = "show";
			_showBackdrop = true;
			//StateHasChanged();
		}

		private void Hide()
		{
			_modalDisplay = "none;";
			_modalClass = "";
			_showBackdrop = false;
			//StateHasChanged();
			Navigation.NavigateTo("/" + Url);
		}
	}
}
