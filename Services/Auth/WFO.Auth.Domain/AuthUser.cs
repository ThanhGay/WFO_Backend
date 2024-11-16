using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Shared.Constant.Database;

namespace WFO.Auth.Domain
{
    [Table(nameof(AuthUser), Schema = DbSchema.Auth)]
    public class AuthUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [MinLength(6)]
        public required string Password { get; set; }
        public string? OTP { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Sex { get; set; }
        public DateTime DateOfBirth { get; set; }

        [Length(10, 10)]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public required string Role { get; set; }
        public bool IsBanned { get; set; } = false;
    }
}
