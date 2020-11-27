using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Configuration;
using Models;

namespace BminingBlazor.Services
{
    public class ClientDataService : IClientDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;

        public ClientDataService(IDataAccess dataAccess,IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
        }
        public async Task<int> CreateClient(ClienteModel client)
        {
            var sql =
                "insert into Cliente (Cliente.Nombre_Cliente)" +
                " Values (@Nombre_Cliente)";
            await _dataAccess.SaveData(sql, client, _configuration.GetConnectionString("default"));
            return 0;

        }

        public async Task<List<ClienteModel>> ReadClient()
        {
            string sql = $"select*from {TableConstants.TablaClientes}";
            var clients = await _dataAccess.LoadData<ClienteModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return clients;
        }
    }
}