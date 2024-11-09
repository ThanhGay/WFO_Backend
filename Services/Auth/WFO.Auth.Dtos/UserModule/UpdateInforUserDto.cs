using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFO.Auth.Dtos.UserModule
{
    public class UpdateInforUserDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Sex { get; set; }
        [Length(10, 10, ErrorMessage ="PhoneNumber have 10 digits")]
        public string? Phone { get; set; }
    }
}
