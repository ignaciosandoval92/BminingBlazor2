using BminingBlazor.ViewModels.User;
using Data;
using System.Collections.Generic;

namespace BminingBlazor.Utility
{
    public static class ViewModelConverter
    {
        public static UserViewModel GetUserViewModel(IDictionary<string, object> user)
        {
            return new UserViewModel
            {
                MyRut = (string)user[UserConstants.Rut],
                MyContractType = (ContractTypeEnum)user[UserConstants.CodContractType],
                MyEmail = (string)user[UserConstants.EmailBmining],
                MyName = (string)user[UserConstants.Name],
                MyPaternalSurname = (string)user[UserConstants.PaternalLastName],
                MyMaternalSurname = (string)user[UserConstants.MaternalLastName],
                MyJob = (string)user[UserConstants.Job],
                MyTelephone = (string)user[UserConstants.Phone],
                MyDirection = (string)user[UserConstants.HomeAddress],
                MyId = (int)user[UserConstants.UserId]
            };
        }
        public static MemberViewModel GetMemberViewModel(IDictionary<string, object> user)
        {
            return new MemberViewModel
            {
                MyRut = (string)user[UserConstants.Rut],
                MyContractType = (ContractTypeEnum)user[UserConstants.CodContractType],
                MyEmail = (string)user[UserConstants.EmailBmining],
                MyName = (string)user[UserConstants.Name],
                MyPaternalSurname = (string)user[UserConstants.PaternalLastName],
                MyMaternalSurname = (string)user[UserConstants.MaternalLastName],
                MyJob = (string)user[UserConstants.Job],
                MyTelephone = (string)user[UserConstants.Phone],
                MyDirection = (string)user[UserConstants.HomeAddress],
                MyId = (int)user[UserConstants.UserId],
                MyProjectHours = (float)user[MemberConstants.ProjectHours],
                MyProjectId = (int)user[MemberConstants.ProjectId],
            };
        }
    }
}
