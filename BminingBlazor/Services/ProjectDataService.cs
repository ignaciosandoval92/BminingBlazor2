using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.User;
using Data;
using Microsoft.Extensions.Configuration;
using Models;
using Models.Project;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Data.TableConstants;
using MemberViewModel = BminingBlazor.ViewModels.User.MemberViewModel;



namespace BminingBlazor.Services
{
    public class ProjectDataService : IProjectDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        public ProjectDataService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("default");
        }
        public async Task<int> CreateProject(ProjectViewModel createProject)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var projectId = await queryFactory.Query(ProjectTable)
                                             .InsertGetIdAsync<int>(new Dictionary<string, object>
                                             {
                                                 {ProjectConstants.ProjectCode,createProject.MyProjectCode},
                                                 {ProjectConstants.ClientId,createProject.MyClientId },
                                                 {ProjectConstants.CodProjectType,createProject.MyProjectType },
                                                 {ProjectConstants.ProjectName,createProject.MyProjectName },
                                                 {ProjectConstants.StartDate,createProject.MyStartDate },
                                                 {ProjectConstants.EndDate,createProject.MyEndDate },
                                                 {ProjectConstants.CreatorId,createProject.MyCreator.MyId },
                                                 {ProjectConstants.ProjectManagerId,createProject.MyProjectManager.MyId },
                                                 {ProjectConstants.StatusId,createProject.MyProjectStatus }
                                             });
            foreach (var member in createProject.OurMembers)
            {
                await queryFactory.Query(MembersTable)
                .InsertAsync(new Dictionary<string, object>
                {
                    {MemberConstants.ProjectId,projectId },
                    {MemberConstants.UserId,member.MyId },
                    {MemberConstants.ProjectHours,member.MyProjectHours }
                });
            }
            foreach (var paymentViewModel in createProject.OurPayments)
            {
                await queryFactory.Query(PaymentTable)
                .InsertAsync(new Dictionary<string, object>
                {
                    {PaymentConstants.ProjectId,projectId },
                    {PaymentConstants.PaymentName,paymentViewModel.MyName },
                    {PaymentConstants.CodPaymentStatusType,1 },
                    {PaymentConstants.InvoiceExpirationDate,paymentViewModel.InvoiceExpirationDate },
                    {PaymentConstants.IssueExpirationDate,paymentViewModel.IssueExpirationDate }
                });
            }

            return projectId;           

        }
       
        // TODO: 
        public async Task<int> EditPaymentStatus(PaymentModel estadopago)
        {
            var sql =
                "Update PaymentStatus" +
                " set PaymentStatus.codPaymentStatus=@Cod_TipoEstadoPago" +
                " where PaymentStatus.codPaymentStatus=@Cod_EstadoPago ";
            await _dataAccess.UpdateData(sql, estadopago, _configuration.GetConnectionString("default"));
            return 1;
        }

        //TODO Insertar creato debe tener este ademas de existir para agregar estados
        public async Task AddPaymentStatus(PaymentModel estadopago)
        {
            var sql =
                "insert into EstadoPago (EstadoPago.Estado_Pago,EstadoPago.Id_Proyecto,EstadoPago.Cod_TipoEstadoPago,EstadoPago.IssueExpirationDate,EstadoPago.InvoiceExpirationDate) " +
                " Values (@Estado_Pago,@Id_Proyecto,@Cod_TipoEstadoPago,@IssueExpirationDate,@InvoiceExpirationDate)";
            await _dataAccess.SaveData(sql, estadopago, _configuration.GetConnectionString("default"));
        }

        public async Task AddMember(MemberViewModel memberViewModel)
        {
            var sql = "insert into Integrantes_Proyecto (Integrantes_Proyecto.Id_Usuario,Integrantes_Proyecto.Id_Proyecto,Integrantes_Proyecto.Project_Hours) " +
                      "Values (@Id_Usuario,@Id_Proyecto,@HoursProject)";
            await _dataAccess.SaveData(sql, memberViewModel, _configuration.GetConnectionString("default"));
        }

        public async Task DeleteMember(int memberId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(MembersTable).Where(MemberConstants.CodMembers, memberId).DeleteAsync();
        }
        public async Task<List<TipoProyectoModel>> ReadProjectType()
        {
            var sql = $"select*from {TableConstants.ProjectTypeTable}";
            var tipro = await _dataAccess.LoadData<TipoProyectoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return tipro;
        }
        public async Task<List<TipoEstadoPagoModel>> ReadPaymentStatusType()
        {
            string sql = $"select*from {TableConstants.PaymentTypeTable}";
            var tipoep = await _dataAccess.LoadData<TipoEstadoPagoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return tipoep;
        }
        //TODO Insertar mover view model a su carpeta
        public async Task<List<ProjectViewModel>> ReadProjects()
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var projects = (await queryFactory
                .Query()
                .From(ProjectTable)
                .Join(UserTable, UserTable + "." + UserConstants.UserId, ProjectTable + "." + ProjectConstants.ProjectManagerId)
                .Join(ClientTable, ClientTable + "." + ClientConstants.ClientId, ProjectTable + "." + ProjectConstants.ClientId)
                .Select(ProjectTable + "." + ProjectConstants.ProjectId)
                .Select(ProjectConstants.ProjectCode)
                .Select(ProjectConstants.ProjectName)
                .Select(ProjectConstants.ProjectManagerId)
                .Select(UserConstants.EmailBmining)
                .Select(ClientConstants.ClientName)
                .Select(ProjectConstants.CodProjectType)
                .Select(ProjectConstants.StatusId)
                .Select(ClientTable + "." + ClientConstants.ClientId)
                .GetAsync<ProjectModel>()).ToList();
            var projectViewModel = new List<ProjectViewModel>();
            foreach (var projectModel in projects)
            {
                projectViewModel.Add(new ProjectViewModel
                {
                    MyId = projectModel.ProjectId,
                    MyProjectCode = projectModel.CodProject,
                    MyProjectName = projectModel.ProjectName,
                    MyProjectStatus = (ProjectStatusEnum)projectModel.StatusId,
                    MyProjectType = (ProjectTypeEnum)projectModel.CodProjectType,
                    MyClientId = projectModel.ClientId,
                    MyClientName = projectModel.ClientName,
                    MyProjectManager = new UserViewModel { MyId = projectModel.ProjectManagerId, MyEmail = projectModel.EmailBmining },
                });
            }
            return projectViewModel.ToList();
        }
        public async Task<int> ReadIdProjectManager(int idProject)
        {
            string sql = "select Project.projectManagerId " +
                         $" from {TableConstants.ProjectTable} " +
                         $" where projectId={idProject}";
            var idManager =
                await _dataAccess.LoadData<ProjectModel, dynamic>(sql, new { }, _configuration.GetConnectionString("default"));
            return idManager.First().ProjectManagerId;
        }

        public async Task<List<MemberViewModel>> ReadMembers(int idProject)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var members = (await queryFactory
                .Query()
                .From(UserTable)
                .Join(MembersTable, MembersTable + "." + UserConstants.UserId, UserTable + "." + MemberConstants.UserId)
                .Select(UserTable + "." + UserConstants.UserId)
                .Select(UserConstants.EmailBmining)
                .Select(UserConstants.Name)
                .Select(UserConstants.PaternalLastName)
                .Select(UserConstants.MaternalLastName)
                .Select(UserConstants.Rut)
                .Select(UserConstants.Job)
                .Select(UserConstants.Phone)
                .Select(UserConstants.HomeAddress)
                .Select(MemberConstants.CodMembers)
                .Where(MembersTable + "." + MemberConstants.ProjectId, idProject)
                .GroupBy(UserConstants.Name)
                .GetAsync<UserModel>()).ToList();
            var membersViewModel = new List<MemberViewModel>();
            foreach(var member in members)
            {
                membersViewModel.Add(new MemberViewModel
                {
                    MyCodMember = member.CodMembers,
                    MyDirection = member.HomeAdress,
                    MyEmail = member.EmailBmining,
                    MyId = member.UserId,
                    MyJob = member.Job,
                    MyMaternalSurname = member.MaternalLastName,
                    MyName = member.Name,
                    MyPaternalSurname = member.PaternalLastName

                });
            }
            return membersViewModel;

       
        }
    

        public async Task DeleteProject(int projectId)
        {
            string sql = "Delete " +
                         $" from {TableConstants.ProjectTable} " +
                         $" where Proyecto.Id_Proyecto=@id_proyecto";
            await _dataAccess.DeleteData(sql, new { Id_Proyecto = projectId },
                _configuration.GetConnectionString("default"));
        }





        public async Task<List<ProjectViewModel>> ReadProjectsOwnedByUser(int userId)
        {
            //string sql =
            //    "select Proyecto.Id_Proyecto,Proyecto.Cod_Proyecto,Proyecto.Nombre_Proyecto,(Usuario.Email_Bmining) as Email_JefeProyecto,Cliente.Nombre_Cliente,Tipo_Proyecto.Tipo_Proyecto,(EstadoPago.Estado_Pago) as Tipo_Pago,Tipo_EstadoPago.TipoEstadoPago,EstadoPago.Cod_EstadoPago " +
            //    $"from {TableConstants.TablaProyecto},{TableConstants.TablaTipoEstadoPago},{TableConstants.TablaTipoProyecto},{TableConstants.TablaEstadoPago},{TableConstants.TablaUsuario},{TableConstants.TablaClientes} " +
            //    $"where Proyecto.Id_Proyecto=EstadoPago.Id_Proyecto " +
            //    $"and EstadoPago.Cod_TipoEstadoPago=Tipo_EstadoPago.Cod_TipoEstadoPago " +
            //    $"and Proyecto.Cod_TipoProyecto=Tipo_Proyecto.Cod_TipoProyecto " +
            //    $"and Usuario.Id=Proyecto.Id_JefeProyecto " +
            //    $"and Proyecto.Id_Cliente=Cliente.Id_Cliente";
            //var projectViewModels = await _dataAccess.LoadData<ViewProyectoModel, dynamic>(sql, new { },
            //        _configuration.GetConnectionString("default"));
            //return projectViewModels;
            return await Task.Run(() => new List<ProjectViewModel>());
        }

        public async Task<List<StatusProjectModel>> GetAvailableProjectStatus()
        {
            string sql = $"select*from {TableConstants.StatusProjectTable}";
            var statusProject = await _dataAccess.LoadData<StatusProjectModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return statusProject;
        }
        public async Task<ProjectViewModel> ReadProject(int projectId)
        {
            string sql = $"select*from{TableConstants.ProjectTable}" +
                         $" where Project.projectId=@projectId";
            var project = await _dataAccess.LoadData<ProjectViewModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return project.First();
        }

        public async Task AddPaymentStatus(PaymentViewModel payment)
        {
            await Task.Run(() => 1);
        }

        public async Task<int> EditPaymentStatus(PaymentViewModel editPayment)
        {
            return await Task.Run(() => 1);
        }
    }
}
