using System.Collections.Generic;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.User;

namespace BminingBlazor.Services
{
    public interface IFranciscoProjectDataService
    {
        Task<List<ProjectViewModel>> ReadProjectsOwnedByUser(int userId);
    }
}