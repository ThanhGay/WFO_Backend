using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Auth.Dtos.UserModule
{
    public class LoginDto
    {
        private string _email;
        public required string Email { get => _email; set => _email = value.Trim(); }
        public string? Password { get; set; }
    }
}
