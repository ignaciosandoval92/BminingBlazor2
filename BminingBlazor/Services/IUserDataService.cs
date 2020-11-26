using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace BminingBlazor.Services
{
    public interface IUserDataService
    {
        Task<List<UsuarioModel>> ReadUsers();
        
        Task<int> CreateUser(UsuarioModel usuario);
        Task EditUser(MemberProjectEditModel memberProject);
        Task<MemberProjectEditModel> EditUsers(string id);
        Task<List<ContratoModel>> ReadContrato();
       
        Task<List<MemberProjectEditModel>> ReadUser(int id);
        Task DeleteUser(int id);
        Task<int> GetUserId(string email);



    }
}
