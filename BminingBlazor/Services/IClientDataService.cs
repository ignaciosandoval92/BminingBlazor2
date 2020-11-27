using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace BminingBlazor.Services
{
    public interface IClientDataService
    {
        Task<int> CreateClient(ClienteModel client);
        Task<List<ClienteModel>> ReadClient();
    }
}