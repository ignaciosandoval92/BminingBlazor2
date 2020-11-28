using BminingBlazor.ViewModels.User;
using Data;
using Microsoft.Extensions.Configuration;
using Models;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Data.TableConstants;

namespace BminingBlazor.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public UserDataService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("default");
        }
        public async Task<List<UsuarioModel>> ReadUsers()
        {
            string sql = "select Usuario.Id, Usuario.Email_Bmining,Usuario.Nombre ,Usuario.Apellido_Paterno,Usuario.Apellido_Materno,Usuario.Rut,Usuario.Cargo,Usuario.Telefono,Usuario.Direccion ,Contrato.TipoContrato as Cod_TipoContrato " +
                         $"from {UserTable},{ContractTable} " +
                         " where Usuario.Cod_TipoContrato=Contrato.Cod_TipoContrato;";
            var users = await _dataAccess.LoadData<UsuarioModel, dynamic>(sql, new { }, _configuration.GetConnectionString("default"));
            return users;
        }



        public async Task<List<ContratoModel>> ReadContrato()
        {
            string sql = $"select*from {ContractTable}";
            var tc = await _dataAccess.LoadData<ContratoModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));
            return tc;
        }

        public async Task<List<MemberProjectEditModel>> ReadUser(int id)
        {
            string sql = "select * " +
                         $" from {UserTable}" +
                         $" where Usuario.Id={id}";
            var user = await _dataAccess.LoadData<MemberProjectEditModel, dynamic>(sql, new { },
                   _configuration.GetConnectionString("default"));
            return user;
        }

        public async Task<List<UserViewModel>> ReadUsers(IEnumerable<int> ids)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var users = (await queryFactory.Query(UserTable).Where(UserConstants.UserId, ids).GetAsync<UserModel>()).ToList();

            var userViewModels = new List<UserViewModel>();
            foreach (var userModel in users)
            {
                userViewModels.Add(new UserViewModel
                {
                    MyContractType = (ContractTypeEnum) userModel.Cod_TipoContrato,
                    MyDirection = userModel.Direccion,
                    MyEmail = userModel.Email_Bmining,
                    MyId = userModel.id,
                    MyJob = userModel.Cargo,
                    MyMaternalSurname = userModel.Apellido_Materno,
                    MyName = userModel.Nombre,
                    MyPaternalSurname = userModel.Apellido_Paterno,
                    MyRut = userModel.Rut,
                    MyTelephone = userModel.Telefono
                });
            }
            return userViewModels;
        }



        public async Task<int> CreateUser(UsuarioModel usuario)
        {
            var sql =
                "insert into Usuario (Usuario.Email_Bmining,Usuario.Nombre,Usuario.Apellido_Paterno,Usuario.Apellido_Materno,Usuario.Rut) " +
                "values (@Email_Bmining,@Nombre,@Apellido_Paterno,@Apellido_Materno,@Rut)";
            await _dataAccess.SaveData(sql, usuario, _configuration.GetConnectionString("default"));

            sql =
                "select Usuario.Id " +
                $"from {UserTable} " +
                $"  where Usuario.Email_Bmining = '{usuario.Email_Bmining}';";

            //    $"where Usuario.Email_Bmining={usuario.Email_Bmining};";
            var items = await _dataAccess.LoadData<UsuarioModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));

            return items.First().id;

        }

        public async Task<int> GetUserId(string email)
        {
            string sql =
                "select Usuario.Id " +
                $"from {UserTable} " +
                $"  where Usuario.Email_Bmining = '{email}';";


            var items = await _dataAccess.LoadData<UsuarioModel, dynamic>(sql, new { },
                _configuration.GetConnectionString("default"));

            return items.First().id;
        }



        public async Task EditUser(MemberProjectEditModel usuario2)
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
                         $"from {UserTable} " +
                         $"where Usuario.Id=@Id";
            await _dataAccess.DeleteData(sql, new { Id = id }, _configuration.GetConnectionString("default"));

        }

        public Task<MemberProjectEditModel> EditUsers(string id)
        {
            throw new System.NotImplementedException();
        }

    }
}