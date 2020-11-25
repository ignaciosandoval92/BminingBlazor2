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
        Task EditUser(UsuarioEditModel usuario);
        Task<UsuarioEditModel> EditUsers(string id);
        Task<List<ContratoModel>> ReadContrato();
       
        Task<List<UsuarioEditModel>> ReadUser(int id);
        Task DeleteUser(int id);
        Task<int> IdUserFromEmail(string email);



    }
}
