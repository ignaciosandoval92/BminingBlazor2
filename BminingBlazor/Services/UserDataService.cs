using BminingBlazor.ViewModels.User;
using Data;
using Microsoft.Extensions.Configuration;
using Models;
using SqlKata.Execution;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Data.TableConstants;
using UserModel = Models.UserModel;

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
        public async Task<List<UserViewModel>> ReadUsers()
        {
            string sql = "select User.UserId as MyID, User.EmailBmining as MyEmail,User.Name as MyName ,User.PaternalLastName as MyPaternalSurname,User.MaternalLastName as MyMaternalSurname,User.Rut as MyRut,User.Job as MyJob,User.Phone as MyTelephone,User.HomeAdress as MyDirection,User.CodContractType as MyContractType " +
                         $"from {UserTable},{ContractTable} " +
                         " where User.CodContractType=Contract.CodContractType;";
            var users = await _dataAccess.LoadData<UserViewModel, dynamic>(sql, new { }, _configuration.GetConnectionString("default"));
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
                         $" where User.userId={id}";
            var user = await _dataAccess.LoadData<MemberProjectEditModel, dynamic>(sql, new { },
                   _configuration.GetConnectionString("default"));
            return user;
        }

        public async Task<List<UserViewModel>> ReadUsers(IEnumerable<int> ids)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var users = (await queryFactory.Query(UserTable).WhereIn(UserConstants.UserId, ids).GetAsync<UserModel>()).ToList();
            var userViewModels = new List<UserViewModel>();
            foreach (var userModel in users)
            {
                userViewModels.Add(new UserViewModel
                {
                    MyContractType = (ContractTypeEnum)userModel.CodContractType,
                    MyDirection = userModel.HomeAdress,
                    MyEmail = userModel.EmailBmining,
                    MyId = userModel.UserId,
                    MyJob = userModel.Job,
                    MyMaternalSurname = userModel.MaternalLastName,
                    MyName = userModel.Name,
                    MyPaternalSurname = userModel.PaternalLastName,
                    MyRut = userModel.Rut,
                    MyTelephone = userModel.Phone
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
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var clients = await queryFactory.Query(UserTable)
                .Select(UserConstants.UserId)
                .Where(UserConstants.EmailBmining, email)
                                             .GetAsync<int>();
            return clients.First();
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