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
        Task<List<ProjectViewModel>> ReadProjects();
        Task<ProjectViewModel> ReadProject(int projectId);
        Task<int> EditPaymentStatus(PaymentViewModel editPayment);        
        Task DeleteMember(int memberId);
        Task AddMember(List<MemberViewModel> member,int idProject);
        Task DeleteProject(int projectId);
        Task<List<StatusProjectModel>> GetAvailableProjectStatus();
        Task<List<ProjectViewModel>> ReadProjectsOwnedByUser(int userId);
        Task<List<MemberViewModel>> ReadMembers(int idProject);
        Task<List<PaymentViewModel>> ReadPaymentStatusOfProject(int idProject);
        Task AddPaymentStatus(List<PaymentViewModel> payments, int idProject);
        Task<PaymentViewModel> ReadPaymentStatus(int paymentId);
        Task<int> ReadIdProjectManager(int idProject);
    }
}