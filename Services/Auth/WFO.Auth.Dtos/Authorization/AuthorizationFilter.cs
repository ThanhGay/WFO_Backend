using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WFO.Auth.Dtos.Authorization
{
    public class AuthorizationFilter: Attribute, IAuthorizationFilter
    {
        public readonly string Role;
        public AuthorizationFilter(string role) {
            Role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var claims = user.Claims.ToList(); 
            var userTypeClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (userTypeClaim != null)
            {
                string userType = userTypeClaim.Value;
                if (!Role.Contains(userType))
                {
                    context.Result = new UnauthorizedObjectResult(new { message = $"User type = {userType} không có quyền" });
                }
            }
            else
            {
                context.Result = new UnauthorizedObjectResult(new { message = $"Không có quyền" });
            }
        }
    }
}
