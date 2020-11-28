using BminingBlazor.Utility;
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
using static Data.ProjectConstants;
using static Data.TableConstants;

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

            var projectModels = (await queryFactory.Query(ProjectTable)
                                .Where(ProjectManagerId, userId).GetAsync<ProjectModel>()).ToList();

            var listOfProjectModels = new List<ProjectViewModel>();
            foreach (var projectModel in projectModels)
            {
                // Get a Base Project Model
                var projectViewModel = projectModel.GetBaseProjectViewModel();

                // Client 
                projectViewModel.MyClientId = projectModel.Id_Cliente;
                var clientModel = await _clientDataService.ReadClient(projectViewModel.MyClientId);
                projectViewModel.MyClientName = clientModel.Nombre_Cliente;

                // Get Members
                var memberModels = (await queryFactory.Query(MembersTable).Where(MemberConstants.ProjectId, projectViewModel.MyId).GetAsync<MemberModel>()).ToList();
                var userViewModels = await _userDataService.ReadUsers(memberModels.Select(model => model.Id_Usuario));

                foreach (var userViewModel in userViewModels)
                {
                    var memberModel = memberModels.First(model => model.Id_Usuario == userViewModel.MyId);
                    var memberViewModel = new MemberViewModel
                    {
                        MyId = userViewModel.MyId,
                        MyEmail = userViewModel.MyEmail,
                        MyName = userViewModel.MyName,
                        MyPaternalSurname = userViewModel.MyPaternalSurname,
                        MyMaternalSurname = userViewModel.MyMaternalSurname,
                        MyRut = userViewModel.MyRut,
                        MyJob = userViewModel.MyJob,
                        MyTelephone = userViewModel.MyTelephone,
                        MyDirection = userViewModel.MyDirection,
                        MyContractType = userViewModel.MyContractType,
                        MyProjectHours = memberModel.Project_Hours,
                        MyProjectId = memberModel.Id_Proyecto
                    };
                    projectViewModel.OurMembers.Add(memberViewModel);
                }
                //projectViewModel.MyId
                var paymentModels = (await queryFactory.Query(PaymentTable).Where(PaymentConstants.ProjectId,1353 ).GetAsync<PaymentModel>()).ToList();
                
                var paymentViewModels = new List<PaymentViewModel>();
                foreach (var paymentModel in paymentModels)
                {
                    var paymentViewModel = new PaymentViewModel();
                    paymentViewModel.ProjectId = paymentModel.Id_Proyecto;
                    paymentViewModel.Name = paymentModel.
                    paymentViewModel.PaymentStatusType = (PaymentStatusTypeEnum)projectModel.Cod
                    paymentViewModels.Add();
                }

                listOfProjectModels.Add(projectViewModel);
            }
            return listOfProjectModels;
        }

    }
}