using System;

namespace BminingBlazor.ViewModels.Projects
{
    public class SimpleProjectViewModel
    {
        public int MyId { get; set; }
        public string MyProjectCode { get; set; }
        public string MyProjectName { get; set; }
        public ProjectStatusEnum MyProjectStatus { get; set; }
        public ProjectTypeEnum MyProjectType { get; set; }
        public DateTime MyStartDate { get; set; }
        public DateTime MyEndDate { get; set; }
        public int MyClientId { get; set; }
        public int MyProjectManagerId { get; set; }
        public int MyCreatorId { get; set; }
    }
}