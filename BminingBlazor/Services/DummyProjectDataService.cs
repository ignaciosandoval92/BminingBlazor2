using BminingBlazor.ViewModels.Projects;
using Data;
using Microsoft.Extensions.Configuration;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata.Extensions;

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
           
            var projectQuery = queryFactory.Query(TableConstants.ProjectTable).Where(ProjectConstants.ProjectManagerId, userId);
            var  paymentQuery =  queryFactory.Query(TableConstants.PaymentTable);
            var membersQuery = queryFactory.Query(TableConstants.MembersTable).Join(TableConstants.UserTable,$"{UserConstants.UserId}", $"{MemberConstants.UserId}");
            var usersQuery = queryFactory.Query(TableConstants.UserTable);

 

            var items =await projectQuery
                            .IncludeMany(TableConstants.PaymentTable, paymentQuery,ProjectConstants.ProjectId, PaymentConstants.ProjectId)
                            .IncludeMany(TableConstants.MembersTable, membersQuery,ProjectConstants.ProjectId, MemberConstants.ProjectId)
                   .GetAsync();
        

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