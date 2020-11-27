using System.Collections.Generic;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.Project;
using Models;
using ProjectModel = Models.ProjectModel;

namespace BminingBlazor.Services
{
    public interface IProjectDataService
    {
        Task<int> CreateProject(ViewModels.Project.CreateProjectViewModel project);
        Task AddMember(CreateProjectViewModel project);
        Task<List<TipoProyectoModel>> ReadProjectType();
        Task<List<TipoEstadoPagoModel>> ReadPaymentStatusType();
        Task AddPaymentStatus(EstadoPagoModel estadopago);
        Task<int> AddProjectCreator(ProjectModel createProjectView);
        Task<int> AddJefeProyecto(ProjectModel createProjectView);
        Task<int> AddCliente(ProjectModel createProjectView);
        Task<List<ViewProyectoModel>> ReadProjects();
        Task<int> ReadIdProjectManager(int id_proyecto);
        Task<List<MemberProjectEditModel>> ReadMembers(int idProject);
        Task<int> EditPaymentStatus(EstadoPagoModel estadopago);
        Task DeleteMember(int memberId);
        Task DeleteProyecto(int id_proyecto);
        Task<int> CreateCliente(ClienteModel cliente);
        Task<List<ClienteModel>> ReadCliente();
        Task<List<ViewProyectoModel>> ReadProjectsByUser(int userId);
        Task<List<StatusProjectModel>> GetAvailableProjectStatus();
    }
}