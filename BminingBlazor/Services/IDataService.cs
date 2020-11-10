using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;

namespace BminingBlazor.Services
{
    public interface IDataService
    {
        Task<List<UsuarioModel>> ReadUsers();
        Task InsertUsers(UsuarioModel usuario);
        Task EditUsers(UsuarioEditModel usuario);


    }
}
