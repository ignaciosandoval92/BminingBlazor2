using Data;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.User;
using Models.Project;
using MemberViewModel = BminingBlazor.ViewModels.User.MemberViewModel;
using static Data.TableConstants;
using SqlKata;
using SqlKata.Execution;



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
                                                 {ProjectConstants.CodProject,createProject.MyProjectCode},
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
            foreach (var payments in createProject.OurPayments)
            {
                await queryFactory.Query(PaymentTable)
                .InsertAsync(new Dictionary<string, object>
                {
                    {PaymentConstants.ProjectId,projectId },
                    {PaymentConstants.PaymentName,payments.MyName },
                    {PaymentConstants.CodPaymentStatusType,1 },
                    {PaymentConstants.InvoiceExpirationDate,payments.InvoiceExpirationDate },
                    {PaymentConstants.IssueExpirationDate,payments.IssueExpirationDate }
                });
            }

            return projectId;

        }

        public async Task<int> EditPaymentStatus(PaymentViewModel paymentStatus)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var userId = await queryFactory.Query()
                .From(PaymentTable)
                .Where(PaymentConstants.PaymentId, paymentStatus.Id)
                .UpdateAsync(new Dictionary<string, object>{
                { PaymentConstants.CodPaymentStatusType,paymentStatus.PaymentStatusType}
                
        });
     
            return 1;

        }

   

        public async Task AddMember(List<MemberViewModel> members, int idProject)
        {

            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            foreach (var member in members)
            {
                await queryFactory.Query(MembersTable)
                .InsertAsync(new Dictionary<string, object>
                {
                    {MemberConstants.ProjectId,idProject },
                    {MemberConstants.UserId,member.MyId },
                    {MemberConstants.ProjectHours,member.MyProjectHours }
                });
            }
        }


        public async Task<List<ProjectViewModel>> ReadProjects()
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var projects = (await queryFactory
                .Query()
                .From(ProjectTable)
                .Join(UserTable, UserTable + "." + UserConstants.UserId, ProjectTable + "." + ProjectConstants.ProjectManagerId)
                .Join(ClientTable, ClientTable + "." + ClientConstants.ClientId, ProjectTable + "." + ProjectConstants.ClientId)
                .Select(ProjectTable + "." + ProjectConstants.ProjectId)
                .Select(ProjectConstants.CodProject)
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
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var project = (await queryFactory
                .Query()
                .From(ProjectTable)
                .Select(ProjectConstants.ProjectManagerId)               
                .Where(ProjectConstants.ProjectId, idProject)
                .GetAsync<ProjectModel>()).First();
          
            return project.ProjectManagerId;        
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
            foreach (var member in members)
            {
                membersViewModel.Add(new MemberViewModel
                {
                    MyCodMember = member.CodMembers,
                    MyDirection = member.HomeAddress,
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
        public async Task DeleteMember(int memberId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(MembersTable).Where(MemberConstants.CodMembers, memberId).DeleteAsync();


        }

        public async Task DeleteProject(int projectId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(ProjectTable).Where(ProjectConstants.ProjectId, projectId).DeleteAsync();

        }





        public async Task<List<ProjectViewModel>> ReadProjectsOwnedByUser(int userId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var projects = (await queryFactory
                .Query()
                .From(ProjectTable)
                .Join(UserTable, UserTable + "." + UserConstants.UserId, ProjectTable + "." + ProjectConstants.ProjectManagerId)
                .Join(ClientTable, ClientTable + "." + ClientConstants.ClientId, ProjectTable + "." + ProjectConstants.ClientId)
                .Join(MembersTable,MembersTable+"."+MemberConstants.ProjectId, ProjectTable + "." + ProjectConstants.ProjectId)
                .Select(ProjectTable + "." + ProjectConstants.ProjectId)
                .Select(ProjectConstants.CodProject)
                .Select(ProjectConstants.ProjectName)
                .Select(ProjectConstants.ProjectManagerId)
                .Select(UserConstants.EmailBmining)
                .Select(ClientConstants.ClientName)
                .Select(ProjectConstants.CodProjectType)
                .Select(ProjectConstants.StatusId)
                .Select(ClientTable + "." + ClientConstants.ClientId)
                .Where(MembersTable+"."+MemberConstants.UserId,userId)
                .GroupBy(ProjectTable+"."+ProjectConstants.ProjectId)
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

        public async Task<List<StatusProjectModel>> GetAvailableProjectStatus()
        {
            string sql = $"select*from {TableConstants.StatusProjectTable}";
            var statusProject = await _dataAccess.LoadData<StatusProjectModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return statusProject;
        }
        public async Task<ProjectViewModel> ReadProject(int projectId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var projects = (await queryFactory
                .Query()
                .From(ProjectTable)
                .Join(UserTable, UserTable + "." + UserConstants.UserId, ProjectTable + "." + ProjectConstants.ProjectManagerId)
                .Join(ClientTable, ClientTable + "." + ClientConstants.ClientId, ProjectTable + "." + ProjectConstants.ClientId)
                .Select(ProjectTable + "." + ProjectConstants.ProjectId)
                .Select(ProjectConstants.CodProject)
                .Select(ProjectConstants.ProjectName)
                .Select(ProjectConstants.ProjectManagerId)
                .Select(UserConstants.EmailBmining)
                .Select(ClientConstants.ClientName)
                .Select(ProjectConstants.CodProjectType)
                .Select(ProjectConstants.StatusId)
                .Select(ClientTable + "." + ClientConstants.ClientId)
                .Where(ProjectTable+"."+ProjectConstants.ProjectId,projectId)
                .GetAsync<ProjectModel>()).First();
            var projectViewModel = new ProjectViewModel()
            {
                MyId = projectId,
                MyProjectCode = projects.CodProject,
                MyProjectName = projects.ProjectName,
                MyProjectStatus = (ProjectStatusEnum)projects.StatusId,
                MyProjectType = (ProjectTypeEnum)projects.CodProjectType,
                MyClientId = projects.ClientId,
                MyClientName = projects.ClientName,
                MyProjectManager = new UserViewModel { MyId = projects.ProjectManagerId, MyEmail = projects.EmailBmining },
            };
            return projectViewModel;
         
           
            
        }

        public async Task AddPaymentStatus(List<PaymentViewModel> payments,int idProject)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            foreach (var payment in payments)
            {
                await queryFactory.Query(PaymentTable)
                .InsertAsync(new Dictionary<string, object>
                {
                    {PaymentConstants.ProjectId,idProject },
                    {PaymentConstants.CodPaymentStatusType,1},
                    {PaymentConstants.PaymentName,payment.MyName },
                    {PaymentConstants.InvoiceExpirationDate,payment.InvoiceExpirationDate },
                    {PaymentConstants.IssueExpirationDate,payment.IssueExpirationDate }
                });
            }           
        }

      
        public async Task<List<PaymentViewModel>> ReadPaymentStatus(int idProject)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var payments = (await queryFactory
                .Query()
                .From(PaymentTable)
                .Join(PaymentTypeTable, PaymentTypeTable + "." + PaymentTypeConstants.CodPaymentStatusType, PaymentTable + "." + PaymentConstants.CodPaymentStatusType)
                .Select(PaymentTable + "." + PaymentConstants.CodPaymentStatusType)
                .Select(PaymentConstants.PaymentName)
                .Select(PaymentConstants.ProjectId)                
                .Select(PaymentConstants.InvoiceExpirationDate)
                .Select(PaymentConstants.IssueExpirationDate)
                .Select(PaymentConstants.PaymentId)
                .Where(PaymentTable + "." + PaymentConstants.ProjectId, idProject)
                .GroupBy(PaymentConstants.PaymentId)
                .GetAsync<PaymentModel>()).ToList();
            var paymentsViewModel = new List<PaymentViewModel>();
            foreach (var payment in payments)
            {
                paymentsViewModel.Add(new PaymentViewModel
                {
                    MyName = payment.PaymentName,
                    MyProjectId = payment.ProjectId,
                    PaymentStatusType = (PaymentStatusTypeEnum)payment.CodPaymentStatusType,
                    Id = payment.PaymentId,
                    InvoiceExpirationDate = payment.InvoiceExpirationDate,
                    IssueExpirationDate = payment.IssueExpirationDate
                });
            }
            return paymentsViewModel;
        }
        public async Task<PaymentViewModel> ReadPaymentStatu(int paymentId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var payment = (await queryFactory
                .Query()
                .From(PaymentTable)
                .Join(PaymentTypeTable, PaymentTypeTable + "." + PaymentTypeConstants.CodPaymentStatusType, PaymentTable + "." + PaymentConstants.CodPaymentStatusType)
                .Select(PaymentTable + "." + PaymentConstants.CodPaymentStatusType)
                .Select(PaymentConstants.PaymentName)
                .Select(PaymentConstants.ProjectId)
                .Select(PaymentConstants.InvoiceExpirationDate)
                .Select(PaymentConstants.IssueExpirationDate)
                .Select(PaymentConstants.PaymentId)
                .Where(PaymentTable + "." + PaymentConstants.PaymentId, paymentId)
                .GroupBy(PaymentConstants.PaymentId)
                .GetAsync<PaymentModel>()).First();
            var paymentViewModel = new PaymentViewModel()        
           
               {
                    MyName = payment.PaymentName,
                    MyProjectId = payment.ProjectId,
                    PaymentStatusType = (PaymentStatusTypeEnum)payment.CodPaymentStatusType,
                    Id = payment.PaymentId,
                    InvoiceExpirationDate = payment.InvoiceExpirationDate,
                    IssueExpirationDate = payment.IssueExpirationDate
                };
            
            return paymentViewModel;
        }

    }
}
