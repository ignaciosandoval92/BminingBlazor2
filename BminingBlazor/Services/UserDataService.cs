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
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var users = (await queryFactory
                .Query()
                .From(UserTable)
                .Select(UserConstants.UserId)
                .Select(UserConstants.EmailBmining)
                .Select(UserConstants.Name)
                .Select(UserConstants.PaternalLastName)
                .Select(UserConstants.MaternalLastName)
                .Select(UserConstants.Rut)
                .Select(UserConstants.Job)
                .Select(UserConstants.Phone)
                .Select(UserConstants.HomeAddress)
                .Select(UserConstants.CodContractType)
                .GetAsync<UserModel>()).ToList();
            var usersViewModel = new List<UserViewModel>();
            foreach (var user in users)
            {
                usersViewModel.Add(new UserViewModel
                {
                    MyId = user.UserId,
                    MyEmail = user.EmailBmining,
                    MyName = user.Name,
                    MyPaternalSurname = user.PaternalLastName,
                    MyMaternalSurname = user.MaternalLastName,
                    MyRut = user.Rut,
                    MyJob = user.Job,
                    MyTelephone = user.Phone,
                    MyDirection = user.HomeAddress,
                    MyContractType = (ContractTypeEnum)user.CodContractType

                });
            }
            return usersViewModel;
        }

        public async Task<UserViewModel> ReadUser(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var user = (await queryFactory
                .Query()
                .From(UserTable)
                .Select(UserConstants.UserId)
                .Select(UserConstants.EmailBmining)
                .Select(UserConstants.Name)
                .Select(UserConstants.PaternalLastName)
                .Select(UserConstants.MaternalLastName)
                .Select(UserConstants.Rut)
                .Select(UserConstants.Job)
                .Select(UserConstants.Phone)
                .Select(UserConstants.HomeAddress)
                .Select(UserConstants.CodContractType)
                .Where(UserConstants.UserId, id)
                .GetAsync<UserModel>()).First();
            var userViewModel = new UserViewModel()

            {
                MyId = user.UserId,
                MyEmail = user.EmailBmining,
                MyName = user.Name,
                MyPaternalSurname = user.PaternalLastName,
                MyMaternalSurname = user.MaternalLastName,
                MyRut = user.Rut,
                MyJob = user.Job,
                MyTelephone = user.Phone,
                MyDirection = user.HomeAddress,
                MyContractType = (ContractTypeEnum)user.CodContractType

            };
            return userViewModel;        
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
                    MyDirection = userModel.HomeAddress,
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
        public async Task<int> CreateUser(UserViewModel createUser)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var userId = await queryFactory.Query(UserTable)
                                             .InsertGetIdAsync<int>(new Dictionary<string, object>
                                             {
                                                 {UserConstants.EmailBmining,createUser.MyEmail },
                                                 {UserConstants.Name,createUser.MyName },
                                                 {UserConstants.PaternalLastName,createUser.MyPaternalSurname },
                                                 {UserConstants.MaternalLastName,createUser.MyMaternalSurname },
                                                 {UserConstants.Rut,createUser.MyRut },
                                                 {UserConstants.Job,createUser.MyJob },
                                                 {UserConstants.Phone,createUser.MyTelephone },
                                                 {UserConstants.HomeAddress,createUser.MyDirection },
                                                 {UserConstants.CodContractType,createUser.MyContractType }
                                             });
            return userId;
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



        public async Task EditUser(UserViewModel user)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var userId = await queryFactory.Query()
                .From(UserTable)
                .Where(UserConstants.UserId, user.MyId)
                .UpdateAsync(new Dictionary<string, object>{
                { UserConstants.Job,user.MyJob},
                { UserConstants.Phone,user.MyTelephone},
                { UserConstants.HomeAddress,user.MyDirection},
                { UserConstants.CodContractType,user.MyContractType}
        });
                     
        }

        public async Task DeleteUser(int userId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(UserTable).Where(UserConstants.UserId, userId).DeleteAsync();

        }

        public Task<MemberProjectEditModel> EditUsers(string id)
        {
            throw new System.NotImplementedException();
        }

    }
}