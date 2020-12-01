using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.User;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MemberViewModel = BminingBlazor.ViewModels.User.MemberViewModel;

namespace BminingBlazor.Services
{
    public interface IProjectDataService
    {
        Task<int> CreateProject(ProjectViewModel newProject);
        Task<List<TipoProyectoModel>> ReadProjectType();
        Task<List<TipoEstadoPagoModel>> ReadPaymentStatusType();
        Task<List<ProjectViewModel>> ReadProjects();
        Task<ProjectViewModel> ReadProject(int projectId);
        Task<int> EditPaymentStatus(PaymentViewModel editPayment);
        Task AddPaymentStatus(PaymentViewModel payment);
        Task DeleteMember(int memberId);
        Task AddMember(MemberViewModel memberViewModel);
        Task DeleteProject(int projectId);
        Task<List<StatusProjectModel>> GetAvailableProjectStatus();
        Task<List<ProjectViewModel>> ReadProjectsOwnedByUser(int userId);
        Task<List<MemberViewModel>> ReadMembers(int idProject);
    }
}