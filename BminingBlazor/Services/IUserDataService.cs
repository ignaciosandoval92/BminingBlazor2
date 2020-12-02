using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.User;
using Models;

namespace BminingBlazor.Services
{
    public interface IUserDataService
    {
        Task<List<UserViewModel>> ReadUsers();

        Task<int> CreateUser(UserViewModel createUser);
        Task EditUser(UserViewModel user);
        Task<MemberProjectEditModel> EditUsers(string id);
        
        Task<UserViewModel> ReadUser(int id);
        Task DeleteUser(int id);
        Task<int> GetUserId(string email);
        Task<List<UserViewModel>> ReadUsers(IEnumerable<int> ids);
    }
}
