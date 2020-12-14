using System.Diagnostics;

namespace BminingBlazor.ViewModels.User
{
    [DebuggerDisplay("{MyEmail}")]
    public class UserViewModel
    {
        public int MyId { get; set; }
        public string MyEmail { get; set; }
        public string MyName { get; set; }
        public string MyPaternalSurname { get; set; }
        public string MyMaternalSurname { get; set; }
        public string MyRut { get; set; }
        public string MyJob { get; set; }
        public string MyTelephone { get; set; }
        public string MyAddress { get; set; }
        public ContractTypeEnum MyContractType { get; set; }
        
    }
}
