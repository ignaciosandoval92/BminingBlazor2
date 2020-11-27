using BminingBlazor.ViewModels.Projects;
using Data;
using Microsoft.Extensions.Configuration;
using Models.Project;
using System.Collections.Generic;
using System.Threading.Tasks;
using BminingBlazor.Utility;

namespace BminingBlazor.Services
{
    public class DummyProjectDataService : IDummyProjectDataService
    {

        private readonly IDataAccess _dataAccess;
        private readonly IClientDataService _clientDataService;
        private readonly string _connectionString;

        public DummyProjectDataService(IDataAccess dataAccess, 
                                       IConfiguration configuration,
                                       IClientDataService clientDataService)
        {
            _connectionString = configuration.GetConnectionString("default");
            _dataAccess = dataAccess;
            _clientDataService = clientDataService;
        }

        public async Task<List<ProjectViewModel>> GetProjectsOwnedById(int userId)
        {
            var sql = $"SELECT * FROM {TableConstants.TablaProyecto}";
            var projectModels = await _dataAccess.LoadData<ProjectModel, dynamic>(sql, new { }, _connectionString);


            var listOfProjectModels = new List<ProjectViewModel>();
            foreach (var projectModel in projectModels)
            {
                // Get a Base Project Model
                var projectViewModel = projectModel.GetBaseProjectViewModel();

                // Client 
                projectViewModel.ClientId = projectModel.Id_Cliente;
                var clientModel = await _clientDataService.ReadClient(1);
                projectViewModel.ClientName = _connectionString;
            }
            return listOfProjectModels;
        }
    }
}