using System;
using BminingBlazor.ViewModels.User;
using System.Collections.Generic;

namespace BminingBlazor.ViewModels.Projects
{
    public class ProjectViewModel
    {
        public int MyId { get; set; }
        public string MyProjectCode { get; set; }
        public string MyProjectName { get; set; }
        public ProjectStatusEnum MyProjectStatus { get; set; }
        public ProjectTypeEnum MyProjectType { get; set; }
        public List<PaymentViewModel> OurPayments { get; set; }
        public UserViewModel MyProjectManager { get; set; }
        public UserViewModel MyCreator { get; set; }
        public List<MemberViewModel> OurMembers { get; set; }
        public string MyClientName { get; set; }
        public int MyClientId { get; set; }
        public DateTime MyStartDate { get; set; }
        public DateTime MyEndDate { get; set; }
       

        public ProjectViewModel()
        {
            OurPayments = new List<PaymentViewModel>();
            OurMembers = new List<MemberViewModel>();
        }
    }
}
