﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AOPFilterApp.Filter
{
    public class ResultFilter : Attribute, IResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            // 在结果执行之后调用的操作...
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 在结果执行之前调用的一系列操作
        }
    }
}
