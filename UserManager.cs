using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogSystem.Dto;
using BlogSystem.IBLL;
using BlogSystem.Models;

namespace BlogSystem.BLL
{
    public class UserManager : IUserManager
    {
        public async Task ChangePassword(string email, string oldPwd, string newPwd)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                if (await userSvc.GetAllAsync().AnyAsync(m => m.Email == email && m.Password == oldPwd))
                {
                    var user = await userSvc.GetAllAsync().FirstAsync(m => m.Email == email);
                    user.Password = newPwd;
                    await userSvc.EditAsync(user);
                }
            }
        }

        public async Task ChangeUserInformation(string email, string siteName, string imagePath)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var user = await userSvc.GetAllAsync().FirstAsync(m => m.Email == email);
                user.SiteName = siteName;
                user.ImagePath = imagePath;
                await userSvc.EditAsync(user);
            }
        }

        public async Task<UserInformationDto> GetUserByEmail(string email)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                if (await userSvc.GetAllAsync().AnyAsync(m => m.Email == email))
                {
                    return await userSvc.GetAllAsync().Where(m => m.Email == email).Select(m => new UserInformationDto()
                    {
                        Id = m.Id,
                        Email = m.Email,
                        ImagePath = m.ImagePath,
                        FansCount = m.FansCount,
                        SiteName = m.SiteName,
                        FocusCount = m.FocusCount
                    }).FirstAsync();
                }
                else
                {
                    throw new ArgumentException("邮箱地址不存在");
                }
            }
        }

        public bool Login(string email, string password,out Guid userId)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                var user = userSvc.GetAllAsync().FirstOrDefaultAsync(m => m.Email == email && m.Password == password);
                user.Wait();
                var data = user.Result;
                if (data == null)
                {
                    userId = new Guid();
                    return false;
                }
                else
                {
                    userId = data.Id;
                    return true;
                }
            }
        }

        public async Task Register(string email, string password)
        {
            using (IDAL.IUserService userSvc = new DAL.UserService())
            {
                await userSvc.CreateAsync(new User()
                {
                    Email = email,
                    Password = password,
                    ImagePath = "default.png",
                    SiteName = "默认的博客"
                });
            }
        }
    }
}
