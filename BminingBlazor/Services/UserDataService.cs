using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Configuration;
using Models;

namespace BminingBlazor.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;

        public UserDataService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
        }
        public async Task<List<UsuarioModel>> ReadUsers()
        {
            string sql = "select Usuario.Id, Usuario.Email_Bmining,Usuario.Nombre ,Usuario.Apellido_Paterno,Usuario.Apellido_Materno,Usuario.Rut,Usuario.Cargo,Usuario.Telefono,Usuario.Direccion ,Contrato.TipoContrato as Cod_TipoContrato " +
                         $"from {TableConstants.TablaUsuario},{TableConstants.TablaContrato} " +
                         " where Usuario.Cod_TipoContrato=Contrato.Cod_TipoContrato;";
            var users = await _dataAccess.LoadData<UsuarioModel, dynamic>(sql, new { }, _configuration.GetConnectionString("default"));
            return users;
        }



        public async Task<List<ContratoModel>> ReadContrato()
        {
            string sql = $"select*from {TableConstants.TablaContrato}";
            var tc = await _dataAccess.LoadData<ContratoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return tc;
        }

        public async Task<List<UsuarioEditModel>> ReadUser(int id)
        {
            string sql = "select * " +
                         $" from {TableConstants.TablaUsuario}" +
                         $" where Usuario.Id={id}";

            var user =
               await _dataAccess.LoadData<UsuarioEditModel, dynamic>(sql, new { },
                   _configuration.GetConnectionString("default"));
            return user;
        }

        public Task<int> GetUserId(string email)
        {
            return Task.Run(() => 1);
        }


        public async Task<int> CreateUser(UsuarioModel usuario)
        {
            var sql =
                "insert into Usuario (Usuario.Email_Bmining,Usuario.Nombre,Usuario.Apellido_Paterno,Usuario.Apellido_Materno,Usuario.Rut) " +
                "values (@Email_Bmining,@Nombre,@Apellido_Paterno,@Apellido_Materno,@Rut)";
            await _dataAccess.SaveData(sql, usuario, _configuration.GetConnectionString("default"));

            sql =
                "select Usuario.Id " +
                $"from {TableConstants.TablaUsuario} " +
                $"  where Usuario.Email_Bmining = '{usuario.Email_Bmining}';";

            //    $"where Usuario.Email_Bmining={usuario.Email_Bmining};";
            var items = await _dataAccess.LoadData<UsuarioModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));

            return items.First().id;

        }



        public async Task EditUser(UsuarioEditModel usuario2)
        {
            string sql =
                "update Usuario " +
                "set Usuario.Cod_TipoContrato=@Cod_TipoContrato,Usuario.Cargo=@Cargo,Usuario.Telefono=@Telefono,Usuario.Direccion=@Direccion " +
                "where Usuario.Id=@Id;";
            await _dataAccess.UpdateData(sql, usuario2, _configuration.GetConnectionString("default"));
        }

        public async Task DeleteUser(int id)
        {
            string sql = "Delete " +
                         $"from {TableConstants.TablaUsuario} " +
                         $"where Usuario.Id=@Id";
            await _dataAccess.DeleteData(sql, new { Id = id }, _configuration.GetConnectionString("default"));

        }

        public Task<UsuarioEditModel> EditUsers(string id)
        {
            throw new System.NotImplementedException();
        }

    }
}