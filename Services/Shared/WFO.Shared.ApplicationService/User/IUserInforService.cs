using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Shared.ApplicationService.User
{
    public interface IUserInforService
    {
        public bool HasUser(int id);
        public int GetCurrentUserId(IHttpContextAccessor httpContextAccessor);
    }
}
