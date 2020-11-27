using Data;
using Microsoft.Extensions.Configuration;
using Models.Project;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Data.TableConstants;

namespace BminingBlazor.Services
{
    public class ClientDataService : IClientDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;

        public ClientDataService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
        }
        public async Task<int> CreateClient(ClientModel client)
        {
            var sql =
                "insert into Cliente (Cliente.Nombre_Cliente)" +
                " Values (@Nombre_Cliente)";
            await _dataAccess.SaveData(sql, client, _configuration.GetConnectionString("default"));
            return 0;

        }

        public async Task<List<ClientModel>> ReadClients()
        {
            var sql = $"select*from {TablaClientes}";
            var clients = await _dataAccess.LoadData<ClientModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return clients;
        }
        public async Task<ClientModel> ReadClient(int id)
        {
            var sql = $"SELECT * FROM {TablaClientes} " +
                      $"WHERE Cliente.Id_Cliente=@id";

            var clients = await _dataAccess.LoadData<ClientModel, dynamic>(sql, new {  id },
                _configuration.GetConnectionString("default"));
            return clients.First();
        }
    }
}