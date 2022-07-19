//using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practice.Common
{
    //[HtmlTargetElement("CustomPager")]
    //public class CustomPagerTagHelper : TagHelper
    //{

    //    public CustomPagerTagHelper()
    //    { 
        
    //    }

    //    /// <summary>
    //    /// 数据总数，默认为0条
    //    /// </summary>
    //    public int Total { get; set; } = 0;

    //    /// <summary>
    //    /// 当前页码，默认第一页
    //    /// </summary>
    //    public int PageIndex { get; set; } = 1;

    //    /// <summary>
    //    /// 每页记录数，默认20条
    //    /// </summary>
    //    public int PageSize { get; set; } = 20;

    //    /// <summary>
    //    /// 当前页路由
    //    /// </summary>
    //    public string RouteUrl { get; set; }

    //    /// <summary>
    //    /// 当前页的查询条件
    //    /// </summary>
    //    public string Query { get; set; }
          
    //    /// <summary>
    //    /// 共多少页
    //    /// </summary>
    //    public int PageTotal
    //    {
    //        get
    //        {
    //            int mainPageCount = Total / PageSize;
    //            if (Total % PageSize > 0)
    //            {
    //                return mainPageCount + 1;
    //            }
    //            return mainPageCount;
    //        }
    //    }

    //    private string SetQueryString()
    //    {
    //        var result = new List<string>();
    //        if (!string.IsNullOrWhiteSpace(Query))
    //        {
    //            if (Query.StartsWith("?"))
    //            {
    //                Query = Query.Remove(0, 1);
    //            }

    //            string[] paramList = Query.Split('&');
    //            foreach (var param in paramList)
    //            {
    //                var paramName = param.Trim().ToLower();
    //                if (!paramName.StartsWith("pageindex=") && !paramName.StartsWith("pagesize="))
    //                {
    //                    result.Add(param);
    //                }
    //            }
    //            // 用LINQ遍历
    //            // result = paramList.Where(p => !p.ToLower().StartsWith("pageindex=") && !p.ToLower().StartsWith("pagesize=")).ToList();
    //        }
    //        result.Add("pageIndex={0}");
    //        result.Add("pageSize=" + PageSize.ToString());
    //        return "?" + string.Join('&', result);
    //    }

    //    public override void Process(TagHelperContext context, TagHelperOutput output)
    //    {
    //        output.TagName = "div";
    //        output.Attributes.Add("class", "my-pager");

    //        output.PreContent.SetContent($" 共有 {Total} 条记录,每页 {PageSize} 条记录。共 1000 页，当前第 {PageIndex} 页。 ");

    //        if (PageSize <= 0) { PageSize = 20; }
    //        if (PageIndex <= 0) { PageIndex = 1; }
    //        if (Total <= 0) { return; }

    //        //总页数
    //        var totalPage = Total / PageSize + (Total % PageSize > 0 ? 1 : 0);
    //        if (totalPage <= 0) { return; }

    //        Query = SetQueryString();

    //        //构造分页样式
    //        var sbPage = new StringBuilder(string.Empty);

    //        sbPage.Append("<ul class=\"pagination\">");
    //        sbPage.AppendFormat("<li class=\"page-item\"><a class=\"page-link\" href=\"{0}{1}\">首页</a></li>",
    //            RouteUrl,
    //            string.Format(Query, 1)
    //        );

    //        // 计算显示的页码
    //        int start = 1;
    //        int end = totalPage;
    //        bool hasStart = false;
    //        bool hasEnd = false;

    //        if (totalPage > 10)
    //        {
    //            if (PageIndex > 5)
    //            {
    //                start = PageIndex - 4;
    //                hasStart = true;
    //            }

    //            if (start + 9 < totalPage)
    //            {
    //                end = start + 9;
    //                hasEnd = true;
    //            }
    //            else
    //            {
    //                end = totalPage;
    //                start = totalPage - 9;
    //            }
    //        }

    //        if (hasStart)
    //        {
    //            sbPage.AppendFormat("<li class=\"page-item\"><a href=\"{0}{1}\">...</a></li>",
    //                RouteUrl,
    //                string.Format(Query, start - 1)
    //            );
    //        }

    //        for (int i = start; i <= end; i++)
    //        {
    //            sbPage.AppendFormat("<li {1}><a class=\"page-link\" href=\"{2}{3}\">{0}</a></li>",
    //                i,
    //                i == PageIndex ? "class=\"page-item active\"" : "class=\"page-item\"",
    //                RouteUrl,
    //                string.Format(Query, i)
    //            );
    //        }

    //        if (hasEnd)
    //        {
    //            sbPage.AppendFormat("<li class=\"page-item\"><a class=\"page-link\" href=\"{0}{1}\">...</a></li>",
    //                RouteUrl,
    //                string.Format(Query, end + 1)
    //            );
    //        }

    //        sbPage.Append("<li class=\"page-item\">");
    //        sbPage.AppendFormat("<a class=\"page-link\" href=\"{0}{1}\">",
    //                            RouteUrl,
    //                            string.Format(Query, totalPage));
    //        sbPage.Append("尾页");
    //        sbPage.Append("</a>");
    //        sbPage.Append("</li>");
    //        sbPage.Append("</ul>");
    //        output.Content.SetHtmlContent(sbPage.ToString());
    //    }
    //} 
}
