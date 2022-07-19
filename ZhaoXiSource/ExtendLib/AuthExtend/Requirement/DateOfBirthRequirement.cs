using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExtendLib.AuthExtend.Requirement
{
    /// <summary>
    /// DateOfBirth---支持传入
    /// </summary>
    public class DateOfBirthRequirement : IAuthorizationRequirement
    {
    }

    public class DateOfBirthRequirementHandler : AuthorizationHandler<DateOfBirthRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DateOfBirthRequirement requirement)
        {
            if (context.User != null && context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth))
            {
                context.Succeed(requirement);//也可以比较具体规则
            }
            return Task.CompletedTask;
        }
    }
}
