using BminingBlazor.ViewModels.Projects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface IDummyProjectDataService
    {
        Task<List<ProjectViewModel>> GetProjectsOwnedById(int userId);
        Task<List<ProjectResumeViewModel>> GetProjectWhereBelongsUserId(int userId);
    }
}
