using BminingBlazor.ViewModels.User;
using System.Collections.Generic;

namespace BminingBlazor.ViewModels.Projects
{
    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public ProjectStatusEnum ProjectStatus { get; set; }
        public List<PaymentViewModel> Payments { get; set; }
        public UserViewModel ProjectManager { get; set; }
        public UserViewModel Creator { get; set; }
        public List<MemberViewModel> Members { get; set; }
        public string ClientName { get; set; }
        public int ClientId { get; set; }
       

        public ProjectViewModel()
        {
            Payments = new List<PaymentViewModel>();
            Members = new List<MemberViewModel>();
        }
    }
}
