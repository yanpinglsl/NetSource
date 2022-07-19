using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorDemo.Client.Components
{
    public partial class Search
    {
        // 组件、、 自定义控件、、 html 新标签
        public string SearchTerm { get; set; }

        [Parameter]
        public EventCallback<string> OnSearchChanged { get; set; }

        private void SearchChanged()
        {
            OnSearchChanged.InvokeAsync(SearchTerm);
        }
    }
}
