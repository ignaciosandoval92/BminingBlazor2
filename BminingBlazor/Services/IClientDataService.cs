using Models.Project;
using System.Collections.Generic;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.Projects;

namespace BminingBlazor.Services
{
    public interface IClientDataService
    {
        Task<int> CreateClient(string name);
        Task<int> DeleteClient(int id);
        Task<List<ClientViewModel>> ReadClients();
        Task<ClientViewModel> ReadClient(int id);
    }
}