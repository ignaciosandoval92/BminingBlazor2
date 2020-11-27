using Models.Project;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface IClientDataService
    {
        Task<int> CreateClient(ClientModel client);
        Task<List<ClientModel>> ReadClients();
        Task<ClientModel> ReadClient(int id);
    }
}