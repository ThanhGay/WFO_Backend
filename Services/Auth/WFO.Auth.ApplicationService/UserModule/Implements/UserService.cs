using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
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
        private readonly IConfiguration _configuration;

        /// <summary>
        /// send mail
        /// </summary>
        private static int wrongOTP = 0;

        public UserService(
            ILogger<UserService> logger,
            AuthDbContext dbContext,
            Jwtsettings jwtSettings,
            IConfiguration configuration
        )
            : base(logger, dbContext)
        {
            _jwtsettings = jwtSettings;
            _configuration = configuration;
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
                if (existAccount.IsBanned)
                {
                    throw new Exception("Email của bạn đã bị cấm");
                }
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
                        Sex = existAccount.Sex,
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

        public ReturnUserDto UpdateUser(UpdateInforUserDto input, int UserId)
        {
            var existUser = _dbContext.Users.FirstOrDefault(u =>
                u.Id == UserId && u.IsBanned == false
            );
            if (existUser != null)
            {
                existUser.FirstName = input.FirstName;
                existUser.LastName = input.LastName;
                existUser.Sex = input.Sex;
                existUser.DateOfBirth = input.DateOfBirth;
                existUser.Phone = input.Phone;
                existUser.Address = input.Address;

                _dbContext.Users.Update(existUser);
                _dbContext.SaveChanges();

                var result = new ReturnUserDto
                {
                    FirstName = existUser.FirstName,
                    LastName = existUser.LastName,
                    Sex = existUser.Sex,
                    Email = existUser.Email,
                    Address = existUser.Address,
                    DateOfBirth = existUser.DateOfBirth,
                    Phone = existUser.Phone,
                    FullName = (existUser.FirstName + " " + existUser.LastName).Trim(),
                    Type = existUser.Role,
                    Token = "",
                };
                return result;
            }
            else
            {
                throw new Exception("Không tìm thấy tài khoản hoặc tài khoản đã bị cấm");
            }
        }

        public void DeleteUser(int id)
        {
            var existAccount = _dbContext.Users.FirstOrDefault(acc => acc.Id == id);
            if (existAccount != null)
            {
                if (existAccount.Role == "Admin")
                {
                    throw new Exception($"Không thể cấm tài khoản này");
                }
                else
                {
                    existAccount.IsBanned = true;

                    _dbContext.Users.Update(existAccount);
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                throw new Exception("Không tìm thấy tài khoản");
            }
        }

        public void ResetPassword(ResetPasswordDto input)
        {
            var existAccount = _dbContext.Users.FirstOrDefault(u =>
                u.Email == input.Email && u.IsBanned == false
            );
            if (existAccount != null)
            {
                var trueOtp = VerifyOtp(existAccount.Id, input.Otp);
                if (trueOtp)
                {
                    existAccount.OTP = null;
                    existAccount.Password = input.NewPassword;
                    _dbContext.SaveChanges();
                    wrongOTP = 0;
                }
                else
                {
                    wrongOTP += 1;
                    if (wrongOTP > 5)
                    {
                        existAccount.OTP = null;
                        _dbContext.SaveChanges();
                        wrongOTP = 0;
                        throw new Exception("Sai quá 5 lần. Vui lòng thử lại trong giây lát");
                    }
                    throw new Exception($"OTP không chính xác. Còn {6 - wrongOTP} lần");
                }
            }
            else
            {
                throw new Exception("Email chưa được đăng ký tài khoản hoặc đã bị cấm");
            }
        }

        public void SendOtp(RequestOtpDto input)
        {
            Random rd = new Random();

            var existUser = _dbContext.Users.FirstOrDefault(u => u.Email == input.Email);
            if (existUser != null)
            {
                int otpNum = rd.Next(100000, 999999);
                string otpStr = otpNum.ToString();

                SendMail(existUser.Email, "Mã OTP của bạn là: " + otpStr);

                existUser.OTP = otpStr;
                _dbContext.SaveChanges();
                wrongOTP = 0;
            }
            else
            {
                throw new Exception("Email chưa được đăng ký tài khoản");
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

        private void SendMail(string receiveEmail, string body)
        {
            // Tạo một đối tượng MailMessage
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(_configuration["SmtpGmail:senderEmail"]);
            mail.To.Add(receiveEmail);
            mail.Subject = "[C# .NET] Email verification code";
            mail.Body = "Đây là email được gửi từ ASP.Net\n" + body;

            // Tạo một đối tượng SmtpClient
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(
                _configuration["SmtpGmail:senderEmail"],
                _configuration["SmtpGmail:senderPassword"]
            );
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool VerifyOtp(int id, string otp)
        {
            var existUser = _dbContext.Users.FirstOrDefault(u => u.Id == id);
            if (existUser != null)
            {
                return existUser.OTP == otp;
            }
            else
            {
                return false;
            }
        }
    }
}
