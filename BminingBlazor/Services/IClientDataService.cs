using Models.Project;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface IClientDataService
    {
        Task<int> CreateClient(string name);
        Task<int> DeleteClient(int id);
        Task<List<ClientModel>> ReadClients();
        Task<ClientModel> ReadClient(int id);
    }
}