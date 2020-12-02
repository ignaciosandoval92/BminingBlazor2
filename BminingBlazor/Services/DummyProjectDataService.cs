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

            var listOfProjectModels = new List<ProjectViewModel>();

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
                    MyProjectCode = (string)item[ProjectConstants.CodProject],
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
                        MyDirection = (string)user[UserConstants.HomeAddress],
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
                        MyDirection = (string)user[UserConstants.HomeAddress],
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


            return new List<ProjectViewModel>();
            //var projectModels = (await queryFactory.Query(ProjectTable)
            //                    .Where(ProjectManagerId, userId).GetAsync<ProjectModel>()).ToList();

            //var listOfProjectModels = new List<ProjectViewModel>();
            //foreach (var projectModel in projectModels)
            //{
            //    // Get a Base Project Model
            //    var projectViewModel = projectModel.GetBaseProjectViewModel();

            //    // Client 
            //    projectViewModel.MyClientId = projectModel.Id_Cliente;
            //    var clientModel = await _clientDataService.ReadClient(projectViewModel.MyClientId);
            //    projectViewModel.MyClientName = clientModel.Nombre_Cliente;

            //    // Get Members
            //    var memberModels = (await queryFactory.Query(MembersTable).Where(MemberConstants.ProjectId, projectViewModel.MyId).GetAsync<MemberModel>()).ToList();
            //    var userViewModels = await _userDataService.ReadUsers(memberModels.Select(model => model.Id_Usuario));

            //    foreach (var userViewModel in userViewModels)
            //    {
            //        var memberModel = memberModels.First(model => model.Id_Usuario == userViewModel.MyId);
            //        var memberViewModel = new MemberViewModel
            //        {
            //            MyId = userViewModel.MyId,
            //            MyEmail = userViewModel.MyEmail,
            //            MyName = userViewModel.MyName,
            //            MyPaternalSurname = userViewModel.MyPaternalSurname,
            //            MyMaternalSurname = userViewModel.MyMaternalSurname,
            //            MyRut = userViewModel.MyRut,
            //            MyJob = userViewModel.MyJob,
            //            MyTelephone = userViewModel.MyTelephone,
            //            MyDirection = userViewModel.MyDirection,
            //            MyContractType = userViewModel.MyContractType,
            //            MyProjectHours = memberModel.Project_Hours,
            //            MyProjectId = memberModel.Id_Proyecto
            //        };
            //        projectViewModel.OurMembers.Add(memberViewModel);
            //    }
            //    var paymentModels = (await queryFactory.Query(PaymentTable).Where(PaymentConstants.ProjectId,projectViewModel.MyId).GetAsync<PaymentModel>()).ToList();

            //    var paymentViewModels = new List<PaymentViewModel>();
            //    foreach (var paymentModel in paymentModels)
            //    {
            //        var paymentViewModel = new PaymentViewModel
            //        {
            //            ProjectId = paymentModel.Id_Proyecto,
            //            Name = paymentModel.Estado_Pago,
            //            PaymentStatusType = (PaymentStatusTypeEnum) paymentModel.Cod_TipoEstadoPago,
            //            Id = paymentModel.Cod_EstadoPago,
            //            InvoiceExpirationDate = paymentModel.InvoiceExpirationDate,
            //            IssueExpirationDate = paymentModel.IssueExpirationDate
            //        };

            //        paymentViewModels.Add(paymentViewModel);
            //    }
            //    projectViewModel.OurPayments = paymentViewModels;
            //    listOfProjectModels.Add(projectViewModel);
            //}
            //return listOfProjectModels;
        }

    }
}