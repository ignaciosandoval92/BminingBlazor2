using BminingBlazor.Utility;
using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.User;
using Data;
using Microsoft.Extensions.Configuration;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public class DummyProjectDataService : IDummyProjectDataService
    {

        private readonly IDataAccess _dataAccess;
        private readonly IClientDataService _clientDataService;
        private readonly IUserDataService _userDataService;

        private readonly string _connectionString;

        public DummyProjectDataService(IDataAccess dataAccess,
                                       IConfiguration configuration,
                                       IClientDataService clientDataService,
                                       IUserDataService userDataService)
        {
            _connectionString = configuration.GetConnectionString("default");
            _dataAccess = dataAccess;
            _clientDataService = clientDataService;
            _userDataService = userDataService;
        }

        public async Task<List<ProjectViewModel>> GetProjectsOwnedById(int userId)
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
                .Where(ProjectConstants.ProjectManagerId, userId)
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

                PaymentViewModel GetPaymentViewModel(IDictionary<string, object> payment)
                {
                    return new PaymentViewModel
                    {
                        MyProjectId = (int)payment[PaymentConstants.ProjectId],
                        PaymentStatusType = (PaymentStatusTypeEnum)payment[PaymentConstants.CodPaymentStatusType],
                        InvoiceExpirationDate = (DateTime)payment[PaymentConstants.InvoiceExpirationDate],
                        IssueExpirationDate = (DateTime)payment[PaymentConstants.IssueExpirationDate],
                        Id = (int)payment[PaymentConstants.PaymentId],
                        MyName = (string)payment[PaymentConstants.PaymentName]
                    };
                }

                var creator = (IDictionary<string, object>)item[ProjectConstants.Creator];
                project.MyCreator = ViewModelConverter.GetUserViewModel(creator);
                var manager = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
                project.MyProjectManager = ViewModelConverter.GetUserViewModel(manager);

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
                    memberViewModels.Add(ViewModelConverter.GetMemberViewModel(member));
                project.OurMembers = memberViewModels;

                projects.Add(project);
            }
            return projects;
        }

        public async Task<List<ProjectResumeViewModel>> GetProjectWhereBelongsUserId(int userId)
        {

            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var membersQuery = queryFactory.Query(TableConstants.MembersTable)
                .Where(MemberConstants.UserId, userId)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                    $"{TableConstants.MembersTable}.{MemberConstants.ProjectId}");

            var items = (await membersQuery.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var projects = new List<ProjectResumeViewModel>();
            foreach (var item in items)
            {
                var project = new ProjectResumeViewModel()
                {
                    MyProjectId = (int) item[ProjectConstants.ProjectId],
                    MyProjectName = (string) item[ProjectConstants.ProjectName],
                    MyProjectCode = (string) item[ProjectConstants.ProjectCode],
                };

                projects.Add(project);
            }
            return projects;
        }
    }
}