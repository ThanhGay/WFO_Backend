using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Auth.Dtos.UserModule
{
    public class RequestOtpDto
    {
        public required string Email { get; set; }
    }
}
