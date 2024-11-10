using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFO.Auth.Domain;
using WFO.Auth.Dtos.UserModule;

namespace WFO.Auth.ApplicationService.UserModule.Abstracts
{
    public interface IUserService
    {
        public void CreateUser(CreateUserDto input);
        public ReturnUserDto Login(LoginDto input);
        public List<AuthUser> GetAll();
        public void UpdateUser(UpdateInforUserDto input, int UserId);
        public void DeleteUser(int id);
        public void ResetPassword(ResetPasswordDto input);
        public void SendOtp(RequestOtpDto input);
    }
}
