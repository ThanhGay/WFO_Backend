using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using WFO.Auth.ApplicationService.Common;
using WFO.Auth.ApplicationService.UserModule.Abstracts;
using WFO.Auth.Domain;
using WFO.Auth.Dtos.UserModule;
using WFO.Auth.Infrastructure;

namespace WFO.Auth.ApplicationService.UserModule.Implements
{
    public class UserService : AuthServiceBase, IUserService
    {
        private readonly Jwtsettings _jwtsettings;

        public UserService(
            ILogger<UserService> logger,
            AuthDbContext dbContext,
            Jwtsettings jwtSettings
        )
            : base(logger, dbContext)
        {
            _jwtsettings = jwtSettings;
        }

        public List<AuthUser> GetAll()
        {
            return _dbContext.Users.ToList();
        }

        public void CreateUser(CreateUserDto input)
        {
            var existAccount = _dbContext.Users.Any(acc => acc.Email == input.Email);
            if (existAccount)
            {
                throw new Exception($"Email đã được đăng ký");
            }
            else
            {
                var newAccount = new AuthUser
                {
                    Email = input.Email,
                    Password = input.Password,
                    Role = "Customer",
                };

                _dbContext.Users.Add(newAccount);
                _dbContext.SaveChanges();
            }
        }

        public ReturnUserDto Login(LoginDto input)
        {
            var existAccount = _dbContext.Users.FirstOrDefault(acc => acc.Email == input.Email);
            if (existAccount != null)
            {
                if (existAccount.Password == input.Password)
                {
                    var token = CreateToken(existAccount);
                    var result = new ReturnUserDto
                    {
                        Email = existAccount.Email,
                        Token = token,
                        Type = existAccount.Role,
                        FirstName = existAccount.FirstName,
                        LastName = existAccount.LastName,
                        FullName = (existAccount.FirstName + " " + existAccount.LastName).Trim(),
                        Address = existAccount.Address,
                        DateOfBirth = existAccount.DateOfBirth,
                        Phone = existAccount.Phone,
                    };
                    return result;
                }
                else
                {
                    throw new Exception("Mật khẩu không chính xác");
                }
            }
            else
            {
                throw new Exception($"Email chưa được đăng ký");
            }
        }

        private string CreateToken(AuthUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Name, (user.FirstName + " " + user.LastName).Trim()),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.Role),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtsettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtsettings.Issuer,
                audience: _jwtsettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtsettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
