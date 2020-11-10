using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Configuration;
using Models;

namespace BminingBlazor.Services
{
    public class DataService : IDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;

        public DataService(IDataAccess dataAccess,IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
        }
        public async Task<List<UsuarioModel>> ReadUsers()
        {
            string sql = "select Usuario.Email_Bmining,Usuario.Nombre ,Usuario.Apellido_Paterno,Usuario.Apellido_Materno,Usuario.Rut,Usuario.Cargo,Usuario.Telefono,Usuario.Direccion ,Contrato.TipoContrato as Cod_TipoContrato " +
                         $"from {TableConstants.TablaUsuario},{TableConstants.TablaContrato} " +
                         " where Usuario.Cod_TipoContrato=Contrato.Cod_TipoContrato;";
            var users = await _dataAccess.LoadData<UsuarioModel, dynamic>(sql, new { }, _configuration.GetConnectionString("default"));
            return users;
        }

        public async Task InsertUsers(UsuarioModel usuario)
        {
            string sql =
                "insert into Usuario (Usuario.Email_Bmining,Usuario.Nombre,Usuario.Apellido_Paterno,Usuario.Apellido_Materno,Usuario.Rut) " +
                "values (@Email_Bmining,@Nombre,@Apellido_Paterno,@Apellido_Materno,@Rut)";
             await _dataAccess.SaveData(sql,usuario,
                _configuration.GetConnectionString("default"));
            
        }

        public async Task EditUsers(UsuarioEditModel usuario2)
        {
            string sql =
                "update Usuario " +
                "set Usuario.Cod_TipoContrato=@Cod_TipoContrato,Usuario.Cargo=@Cargo,Usuario.Telefono=@Telefono,Usuario.Direccion=@Direccion " +
                "where Usuario.Email_Bmining=@Email_Bmining;";
            await _dataAccess.UpdateData(sql, usuario2, _configuration.GetConnectionString("default"));
        }
    }
}