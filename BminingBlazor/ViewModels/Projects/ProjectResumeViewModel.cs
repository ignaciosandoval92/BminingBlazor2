using System;

namespace BminingBlazor.ViewModels.Projects
{
    public class ProjectResumeViewModel
    {
        public int MyProjectId { get; set; }
        public string MyProjectCode { get; set; }
        public string MyProjectName { get; set; }
        public DateTime MyStartDate { get; set; }
        public DateTime MyEndDate { get; set; }
    }
}