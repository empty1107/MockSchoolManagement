using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MockSchoolManagement.Security
{
    /// <summary>
    /// 只有编辑其他admin角色和声明的处理程序
    /// </summary>
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CanEditOnlyOtherAdminRolesAndClaimsHandler(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequirement requirement)
        {
            //获取http上下文
            HttpContext httpContext = _httpContextAccessor.HttpContext;
            string loggedInAdminId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string adminIdBeingEdited = _httpContextAccessor.HttpContext.Request.Query["userId"];

            //判断用户是否拥有 Admin 角色，并且拥有 claim.Type == "Edit Role"，且值为 true
            if (context.User.IsInRole("Admin") && context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true"))
            {
                //如果当前拥有Admin角色的UserId为空，则说明进入的角色列表页面
                //无需判断当前登录用户的ID
                if (string.IsNullOrEmpty(adminIdBeingEdited))
                {
                    context.Succeed(requirement);
                }
                else if (adminIdBeingEdited.ToLower() != loggedInAdminId.ToLower())
                {
                    //表示成功满足需求
                    context.Succeed(requirement);
                }
            }
            return Task.CompletedTask;
        }
    }
}
