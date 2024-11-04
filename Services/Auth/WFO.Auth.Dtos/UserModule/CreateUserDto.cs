using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Auth.Dtos.UserModule
{
    public class CreateUserDto
    {
        private string _email;

        [
            EmailAddress(ErrorMessage = "Email không đúng định dạng."),
            Required(AllowEmptyStrings = false, ErrorMessage = "Email không được để trống.")
        ]
        public required string Email
        {
            get => _email;
            set => _email = value.Trim();
        }

        public required string Password { get; set; }
    }
}
