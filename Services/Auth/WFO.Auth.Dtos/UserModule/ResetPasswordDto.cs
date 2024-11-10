using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Auth.Dtos.UserModule
{
    public class ResetPasswordDto
    {
        private string _otp;

        [
            EmailAddress(ErrorMessage = "Email không đúng định dạng"),
            Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập email"),
        ]
        public required string Email { get; set; }
        public required string NewPassword { get; set; }

        [
            Length(6, 6, ErrorMessage = "Vui lòng nhập OTP bao gồm 6 ký tự"),
            Required(AllowEmptyStrings = false, ErrorMessage = "Vui lòng nhập OTP"),
        ]
        public string Otp
        {
            get => _otp;
            set => _otp = value.Trim();
        }
    }
}
