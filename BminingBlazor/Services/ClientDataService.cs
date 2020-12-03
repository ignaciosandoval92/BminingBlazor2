using Data;
using Microsoft.Extensions.Configuration;
using Models.Project;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.Projects;
using static Data.ClientConstants;
using static Data.TableConstants;

namespace BminingBlazor.Services
{
    public class ClientDataService : IClientDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public ClientDataService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("default");
        }
        public async Task<int> CreateClient(string name)
        {

            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var clientId = await queryFactory.Query(ClientTable)
                                             .InsertGetIdAsync<int>(new Dictionary<string, object>
                                             {
                                                 {ClientName , name}
                                             });
            return clientId;
        }

        public async Task<int> DeleteClient(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var affected = await queryFactory.Query(ClientTable).Where(ClientId, id).DeleteAsync();
            return affected;
        }
        //TODO cambiar ViewModel
        public async Task<List<ClientViewModel>> ReadClients()
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var clients = (await queryFactory.Query(ClientTable).
                                             GetAsync<ClientModel>()).ToList();
            var clientsViewModel = new List<ClientViewModel>();
            foreach (var client in clients)
            {
                clientsViewModel.Add(new ClientViewModel
                {
                    MyId = client.ClientId,
                    MyName = client.ClientName
                });
            }
            return clientsViewModel;
        }
        public async Task<ClientViewModel> ReadClient(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var client= (await queryFactory
                                .Query(ClientTable)
                                .Where(ClientId, id)
                                .GetAsync<ClientModel>()).First();
            var clientViewModel = new ClientViewModel()
            {
                MyId = client.ClientId,
                MyName = client.ClientName
            };
            return clientViewModel;
        }
    }
}