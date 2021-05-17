using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.User;
using Data;
using Microsoft.Extensions.Configuration;
using Models;
using Models.Project;
using SqlKata.Execution;
using System;
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

            var paymentQuery = queryFactory.Query(TableConstants.PaymentTable);
            var membersQuery = queryFactory.Query(TableConstants.MembersTable)
                .Join(TableConstants.UserTable, $"{TableConstants.UserTable}.{UserConstants.UserId}",
                                                $"{TableConstants.MembersTable}.{MemberConstants.UserId}")
                                                .Select($"{TableConstants.UserTable}.{{*}}",
                                                        $"{TableConstants.MembersTable}.{{{MemberConstants.ProjectId}}}",
                                                        $"{TableConstants.MembersTable}.{{{MemberConstants.ProjectHours}}}");
            var creatorQuery = queryFactory.Query(TableConstants.UserTable);
            var projectManagerQuery = queryFactory.Query(TableConstants.UserTable);

            var projectQuery = queryFactory.Query(TableConstants.ProjectTable)
                .Include(ProjectConstants.Creator, creatorQuery, ProjectConstants.CreatorId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, projectManagerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Join(TableConstants.ClientTable, $"{TableConstants.ClientTable}.{ClientConstants.ClientId}",
                                                  $"{TableConstants.ProjectTable}.{ProjectConstants.ClientId}").
                Select($"{TableConstants.ProjectTable}.{{*}}",
                       $"{TableConstants.ClientTable}.{{{ClientConstants.ClientName}}}");

            var items = (await projectQuery
                            .IncludeMany(TableConstants.PaymentTable, paymentQuery, ProjectConstants.ProjectId, PaymentConstants.ProjectId)
                            .IncludeMany(TableConstants.MembersTable, membersQuery, ProjectConstants.ProjectId, MemberConstants.ProjectId)
                            .GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var projects = new List<ProjectViewModel>();

            foreach (var item in items)
            {
                var project = new ProjectViewModel
                {
                    MyId = (int)item[ProjectConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyClientId = (int)item[ProjectConstants.ClientId],
                    MyClientName = (string)item[ClientConstants.ClientName],
                    MyEndDate = (DateTime)item[ProjectConstants.EndDate],
                    MyStartDate = (DateTime)item[ProjectConstants.StartDate],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],
                    MyProjectStatus = (ProjectStatusEnum)item[ProjectConstants.StatusId],
                    MyProjectType = (ProjectTypeEnum)item[ProjectConstants.CodProjectType],
                };


                UserViewModel GetUserViewModel(IDictionary<string, object> user)
                {
                    return new UserViewModel
                    {
                        MyRut = (string)user[UserConstants.Rut],
                        MyContractType = (ContractTypeEnum)user[UserConstants.CodContractType],
                        MyEmail = (string)user[UserConstants.EmailBmining],
                        MyName = (string)user[UserConstants.Name],
                        MyPaternalSurname = (string)user[UserConstants.PaternalLastName],
                        MyMaternalSurname = (string)user[UserConstants.MaternalLastName],
                        MyJob = (string)user[UserConstants.Job],
                        MyTelephone = (string)user[UserConstants.Phone],
                        MyAddress = (string)user[UserConstants.HomeAddress],
                        MyId = (int)user[UserConstants.UserId]
                    };
                }

                MemberViewModel GetMemberViewModel(IDictionary<string, object> user)
                {
                    return new MemberViewModel
                    {
                        MyRut = (string)user[UserConstants.Rut],
                        MyContractType = (ContractTypeEnum)user[UserConstants.CodContractType],
                        MyEmail = (string)user[UserConstants.EmailBmining],
                        MyName = (string)user[UserConstants.Name],
                        MyPaternalSurname = (string)user[UserConstants.PaternalLastName],
                        MyMaternalSurname = (string)user[UserConstants.MaternalLastName],
                        MyJob = (string)user[UserConstants.Job],
                        MyTelephone = (string)user[UserConstants.Phone],
                        MyAddress = (string)user[UserConstants.HomeAddress],
                        MyId = (int)user[UserConstants.UserId],
                        MyProjectHours = (float)user[MemberConstants.ProjectHours],
                        MyProjectId = (int)user[MemberConstants.ProjectId],
                    };
                }

                PaymentViewModel GetPaymentViewModel(IDictionary<string, object> payment)
                {
                    return new PaymentViewModel
                    {
                        MyProjectId = (int)payment[PaymentConstants.ProjectId],
                        PaymentStatusType = (PaymentStatusTypeEnum)payment[PaymentConstants.CodPaymentStatusType],
                        InvoiceExpirationDate = (DateTime)payment[PaymentConstants.InvoiceExpirationDate],
                        IssueExpirationDate = (DateTime)payment[PaymentConstants.IssueExpirationDate],
                        Id = (int)payment[PaymentConstants.ProjectId]
                    };
                }


                var creator = (IDictionary<string, object>)item[ProjectConstants.Creator];
                project.MyCreator = GetUserViewModel(creator);
                var manager = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
                project.MyProjectManager = GetUserViewModel(manager);

                // Payment
                var payments = (IEnumerable<IDictionary<string, object>>)item[TableConstants.PaymentTable];
                var paymentViewModels = new List<PaymentViewModel>();
                foreach (var payment in payments)
                    paymentViewModels.Add(GetPaymentViewModel(payment));
                project.OurPayments.AddRange(paymentViewModels);

                // Member
                var members = (IEnumerable<IDictionary<string, object>>)item[TableConstants.MembersTable];
                var memberViewModels = new List<MemberViewModel>();
                foreach (var member in members)
                    memberViewModels.Add(GetMemberViewModel(member));
                project.OurMembers = memberViewModels;

                projects.Add(project);
            }
            return projects;
        }

        public async Task<List<SimpleProjectViewModel>> ReadProjectWhereUserBelongs(int userId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var projectQuery = queryFactory.Query(TableConstants.ProjectTable);
            var membersQuery = queryFactory.Query(TableConstants.MembersTable)
                                            .Where(MemberConstants.UserId, userId)
                                            .Include(TableConstants.ProjectTable, projectQuery,
                                                     ProjectConstants.ProjectId, MemberConstants.ProjectId);

            var items = (await membersQuery.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var listOfSimpleProjects = new List<SimpleProjectViewModel>();
            foreach (var item in items)
            {
                var dictionary = (IDictionary<string, object>)item[TableConstants.ProjectTable];
                var simpleProject = new SimpleProjectViewModel
                {
                    MyId = (int)dictionary[ProjectConstants.ProjectId],
                    MyProjectName = (string)dictionary[ProjectConstants.ProjectName],
                    MyClientId = (int)dictionary[ProjectConstants.ClientId],
                    MyEndDate = (DateTime)dictionary[ProjectConstants.EndDate],
                    MyStartDate = (DateTime)dictionary[ProjectConstants.StartDate],
                    MyProjectCode = (string)dictionary[ProjectConstants.ProjectCode],
                    MyProjectStatus = (ProjectStatusEnum)dictionary[ProjectConstants.StatusId],
                    MyProjectType = (ProjectTypeEnum)dictionary[ProjectConstants.CodProjectType],
                    MyCreatorId = (int)dictionary[ProjectConstants.CreatorId],
                    MyProjectManagerId = (int)dictionary[ProjectConstants.ProjectManagerId]
                };
                listOfSimpleProjects.Add(simpleProject);
            }
            return listOfSimpleProjects;
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
                .Join(MembersTable, $"{MembersTable}.{UserConstants.UserId}", $"{UserTable}.{MemberConstants.UserId}")
                .Select($"{UserTable}.{UserConstants.UserId}")
                .Select(UserConstants.EmailBmining)
                .Select(UserConstants.Name)
                .Select(UserConstants.PaternalLastName)
                .Select(UserConstants.MaternalLastName)
                .Select(UserConstants.Rut)
                .Select(UserConstants.Job)
                .Select(UserConstants.Phone)
                .Select(UserConstants.HomeAddress)
                .Select(MemberConstants.CodMembers)
                .Where($"{MembersTable}.{MemberConstants.ProjectId}", idProject)
                .GroupBy(UserConstants.UserId)
                .GetAsync<UserModel>()).ToList();
            var membersViewModel = new List<MemberViewModel>();
            foreach (var member in members)
            {
                membersViewModel.Add(new MemberViewModel
                {
                    MyCodMember = member.CodMembers,
                    MyAddress = member.HomeAddress,
                    MyEmail = member.EmailBmining,
                    MyId = member.UserId,
                    MyJob = member.Job,
                    MyMaternalSurname = member.MaternalLastName,
                    MyName = member.Name,
                    MyPaternalSurname = member.PaternalLastName,
                    MyRut = member.Rut

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
                .Join(UserTable, $"{UserTable}.{UserConstants.UserId}", $"{ProjectTable}.{ProjectConstants.ProjectManagerId}")
                .Join(ClientTable, $"{ClientTable}.{ClientConstants.ClientId}", $"{ProjectTable}.{ProjectConstants.ClientId}")
                .Join(MembersTable, $"{MembersTable}.{MemberConstants.ProjectId}", $"{ProjectTable}.{ProjectConstants.ProjectId}")
                .Select($"{ProjectTable}.{ProjectConstants.ProjectId}")
                .Select(ProjectConstants.ProjectCode)
                .Select(ProjectConstants.ProjectName)
                .Select(ProjectConstants.ProjectManagerId)
                .Select(UserConstants.EmailBmining)
                .Select(ClientConstants.ClientName)
                .Select(ProjectConstants.CodProjectType)
                .Select(ProjectConstants.StatusId)
                .Select($"{ClientTable}.{ClientConstants.ClientId}")
                .Where($"{MembersTable}.{MemberConstants.UserId}", userId)
                .GroupBy($"{ProjectTable}.{ProjectConstants.ProjectId}")
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
                .Join(UserTable, $"{UserTable}.{UserConstants.UserId}", $"{ProjectTable}.{ProjectConstants.ProjectManagerId}")
                .Join(ClientTable, $"{ClientTable}.{ClientConstants.ClientId}", $"{ ProjectTable}.{ProjectConstants.ClientId}")
                .Select($"{ProjectTable}.{ProjectConstants.ProjectId}")
                .Select(ProjectConstants.ProjectCode)
                .Select(ProjectConstants.ProjectName)
                .Select(ProjectConstants.ProjectManagerId)
                .Select(UserConstants.EmailBmining)
                .Select(ClientConstants.ClientName)
                .Select(ProjectConstants.CodProjectType)
                .Select(ProjectConstants.StatusId)
                .Select($"{ClientTable}.{ClientConstants.ClientId}")
                .Where($"{ProjectTable}.{ProjectConstants.ProjectId}", projectId)
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

        public async Task AddPaymentStatus(List<PaymentViewModel> payments, int idProject)
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


        public async Task<List<PaymentViewModel>> ReadPaymentStatusOfProject(int idProject)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var payments = (await queryFactory
                .Query()
                .From(PaymentTable)
                .Join(PaymentTypeTable, $"{PaymentTypeTable}.{PaymentTypeConstants.CodPaymentStatusType}", $"{PaymentTable}.{PaymentConstants.CodPaymentStatusType}")
                .Select($"{PaymentTable}.{PaymentConstants.CodPaymentStatusType}")
                .Select(PaymentConstants.PaymentName)
                .Select(PaymentConstants.ProjectId)
                .Select(PaymentConstants.InvoiceExpirationDate)
                .Select(PaymentConstants.IssueExpirationDate)
                .Select(PaymentConstants.PaymentId)
                .Where($"{PaymentTable}.{PaymentConstants.ProjectId}", idProject)
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
        public async Task<PaymentViewModel> ReadPaymentStatus(int paymentId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var payment = (await queryFactory
                .Query()
                .From(PaymentTable)
                .Join(PaymentTypeTable, $"{PaymentTypeTable}.{PaymentTypeConstants.CodPaymentStatusType}", $"{PaymentTable}.{PaymentConstants.CodPaymentStatusType}")
                .Select($"{PaymentTable}.{PaymentConstants.CodPaymentStatusType}")
                .Select(PaymentConstants.PaymentName)
                .Select(PaymentConstants.ProjectId)
                .Select(PaymentConstants.InvoiceExpirationDate)
                .Select(PaymentConstants.IssueExpirationDate)
                .Select($"{PaymentConstants.PaymentId}")
                .Where($"{PaymentTable}.{PaymentConstants.PaymentId}", paymentId)
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
        public async Task<List<ProjectViewModel>> ReadProjectsOwnedByManager(int managerId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var projects = (await queryFactory
                .Query()
                .From(ProjectTable)
                .Join(UserTable, $"{UserTable}.{UserConstants.UserId}", $"{ProjectTable}.{ProjectConstants.ProjectManagerId}")
                .Join(ClientTable, $"{ClientTable}.{ClientConstants.ClientId}", $"{ProjectTable}.{ProjectConstants.ClientId}")                
                .Select($"{ProjectTable}.{ProjectConstants.ProjectId}")
                .Select(ProjectConstants.ProjectCode)
                .Select(ProjectConstants.ProjectName)
                .Select(ProjectConstants.ProjectManagerId)
                .Select(UserConstants.EmailBmining)
                .Select(ClientConstants.ClientName)
                .Select(ProjectConstants.CodProjectType)
                .Select(ProjectConstants.StatusId)
                .Select($"{ClientTable}.{ClientConstants.ClientId}")
                .Where(ProjectConstants.ProjectManagerId,managerId)
                .GroupBy($"{ProjectTable}.{ProjectConstants.ProjectId}")
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
        public async Task<List<ProjectViewModel>> ReadActiveProjects()
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var paymentQuery = queryFactory.Query(TableConstants.PaymentTable);
            var membersQuery = queryFactory.Query(TableConstants.MembersTable)
                .Join(TableConstants.UserTable, $"{TableConstants.UserTable}.{UserConstants.UserId}",
                                                $"{TableConstants.MembersTable}.{MemberConstants.UserId}")
                                                .Select($"{TableConstants.UserTable}.{{*}}",
                                                        $"{TableConstants.MembersTable}.{{{MemberConstants.ProjectId}}}",
                                                        $"{TableConstants.MembersTable}.{{{MemberConstants.ProjectHours}}}");
            var creatorQuery = queryFactory.Query(TableConstants.UserTable);
            var projectManagerQuery = queryFactory.Query(TableConstants.UserTable);

            var projectQuery = queryFactory.Query(TableConstants.ProjectTable)
                .Include(ProjectConstants.Creator, creatorQuery, ProjectConstants.CreatorId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, projectManagerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Join(TableConstants.ClientTable, $"{TableConstants.ClientTable}.{ClientConstants.ClientId}",
                                                  $"{TableConstants.ProjectTable}.{ProjectConstants.ClientId}").
                Select($"{TableConstants.ProjectTable}.{{*}}",
                       $"{TableConstants.ClientTable}.{{{ClientConstants.ClientName}}}").Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}",(int)(ProjectStatusEnum.Active));

            var items = (await projectQuery
                            .IncludeMany(TableConstants.PaymentTable, paymentQuery, ProjectConstants.ProjectId, PaymentConstants.ProjectId)
                            .IncludeMany(TableConstants.MembersTable, membersQuery, ProjectConstants.ProjectId, MemberConstants.ProjectId)
                            .GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var projects = new List<ProjectViewModel>();

            foreach (var item in items)
            {
                var project = new ProjectViewModel
                {
                    MyId = (int)item[ProjectConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyClientId = (int)item[ProjectConstants.ClientId],
                    MyClientName = (string)item[ClientConstants.ClientName],
                    MyEndDate = (DateTime)item[ProjectConstants.EndDate],
                    MyStartDate = (DateTime)item[ProjectConstants.StartDate],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],
                    MyProjectStatus = (ProjectStatusEnum)item[ProjectConstants.StatusId],
                    MyProjectType = (ProjectTypeEnum)item[ProjectConstants.CodProjectType],
                };


                UserViewModel GetUserViewModel(IDictionary<string, object> user)
                {
                    return new UserViewModel
                    {
                        MyRut = (string)user[UserConstants.Rut],
                        MyContractType = (ContractTypeEnum)user[UserConstants.CodContractType],
                        MyEmail = (string)user[UserConstants.EmailBmining],
                        MyName = (string)user[UserConstants.Name],
                        MyPaternalSurname = (string)user[UserConstants.PaternalLastName],
                        MyMaternalSurname = (string)user[UserConstants.MaternalLastName],
                        MyJob = (string)user[UserConstants.Job],
                        MyTelephone = (string)user[UserConstants.Phone],
                        MyAddress = (string)user[UserConstants.HomeAddress],
                        MyId = (int)user[UserConstants.UserId]
                    };
                }

                MemberViewModel GetMemberViewModel(IDictionary<string, object> user)
                {
                    return new MemberViewModel
                    {
                        MyRut = (string)user[UserConstants.Rut],
                        MyContractType = (ContractTypeEnum)user[UserConstants.CodContractType],
                        MyEmail = (string)user[UserConstants.EmailBmining],
                        MyName = (string)user[UserConstants.Name],
                        MyPaternalSurname = (string)user[UserConstants.PaternalLastName],
                        MyMaternalSurname = (string)user[UserConstants.MaternalLastName],
                        MyJob = (string)user[UserConstants.Job],
                        MyTelephone = (string)user[UserConstants.Phone],
                        MyAddress = (string)user[UserConstants.HomeAddress],
                        MyId = (int)user[UserConstants.UserId],
                        MyProjectHours = (float)user[MemberConstants.ProjectHours],
                        MyProjectId = (int)user[MemberConstants.ProjectId],
                    };
                }

                PaymentViewModel GetPaymentViewModel(IDictionary<string, object> payment)
                {
                    return new PaymentViewModel
                    {
                        MyProjectId = (int)payment[PaymentConstants.ProjectId],
                        PaymentStatusType = (PaymentStatusTypeEnum)payment[PaymentConstants.CodPaymentStatusType],
                        InvoiceExpirationDate = (DateTime)payment[PaymentConstants.InvoiceExpirationDate],
                        IssueExpirationDate = (DateTime)payment[PaymentConstants.IssueExpirationDate],
                        Id = (int)payment[PaymentConstants.ProjectId]
                    };
                }


                var creator = (IDictionary<string, object>)item[ProjectConstants.Creator];
                project.MyCreator = GetUserViewModel(creator);
                var manager = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
                project.MyProjectManager = GetUserViewModel(manager);

                // Payment
                var payments = (IEnumerable<IDictionary<string, object>>)item[TableConstants.PaymentTable];
                var paymentViewModels = new List<PaymentViewModel>();
                foreach (var payment in payments)
                    paymentViewModels.Add(GetPaymentViewModel(payment));
                project.OurPayments.AddRange(paymentViewModels);

                // Member
                var members = (IEnumerable<IDictionary<string, object>>)item[TableConstants.MembersTable];
                var memberViewModels = new List<MemberViewModel>();
                foreach (var member in members)
                    memberViewModels.Add(GetMemberViewModel(member));
                project.OurMembers = memberViewModels;

                projects.Add(project);
            }
            return projects;
        }
    }
}
