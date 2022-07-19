using BlazorDemo.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Components
{
    public partial class UserTable
    {
        [Parameter]
        public int Count { get; set; }

        [Parameter]
        public List<Userinfo> Userinfos { get; set; } = new List<Userinfo>();

        [Parameter]
        public EventCallback<int> OnDeleted { get; set; }

        [Inject]
        public IJSRuntime Js { get; set; }

        // 删除触发的方法
        private async Task Delete(int id)
        {
            var user = Userinfos.FirstOrDefault(p => p.UserID.Equals(id));
            // c#和js互相调用
            var confirmed = await Js.InvokeAsync<bool>("confirm", $"确定要删除用户 {user.UserName}?");
            if (confirmed)
            {
                // 触发业务绑定的事件
                await OnDeleted.InvokeAsync(id);
            }
        }
       

    }
}
