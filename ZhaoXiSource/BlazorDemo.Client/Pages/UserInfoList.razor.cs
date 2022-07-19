using BlazorDemo.Client.Service;
using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Pages
{
    public partial class UserInfoList
    {

        [Inject]
        public IUserHttpRepository userHttpRepository { get; set; }

        public List<Userinfo> Userinfos = new List<Userinfo>(); 
        
        protected override async Task OnInitializedAsync()
        {
            Userinfos = await userHttpRepository.GetUserinfos();
        }

    }
}
