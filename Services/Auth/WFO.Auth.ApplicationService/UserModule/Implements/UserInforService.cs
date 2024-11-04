using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WFO.Auth.ApplicationService.Common;
using WFO.Auth.Infrastructure;
using WFO.Shared.ApplicationService.User;

namespace WFO.Auth.ApplicationService.UserModule.Implements
{
    public class UserInforService : AuthServiceBase, IUserInforService
    {
        public UserInforService(ILogger<UserInforService> logger, AuthDbContext dbContext)
            : base(logger, dbContext) { }

        public bool HasUser(int id)
        {
            return _dbContext.Users.Any(acc => acc.Id == id);
        }

        public int GetCurrentUserId(IHttpContextAccessor httpContextAccessor)
        {
            var claims = httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
            var claim = claims?.FindFirst(JwtRegisteredClaimNames.Sid) ?? claims?.FindFirst("sid");
            if (claim == null)
            {
                throw new Exception(
                    $"Tài khoản không chứa claim \"{System.Security.Claims.ClaimTypes.NameIdentifier}\""
                );
            }
            int userId = int.Parse(claim.Value);
            return userId;
        }
    }
}
