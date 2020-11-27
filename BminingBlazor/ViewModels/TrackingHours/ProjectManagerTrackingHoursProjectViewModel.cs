using System.Collections.Generic;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class ProjectManagerTrackingHoursProjectViewModel
    {
        public int MyProjectId { get; set; }
        public string MyProjectCode { get; set; }
        public string MyProjectName { get; set; }

        public List<ProjectManagerTrackingHoursProjectMemberViewModel> OurProjectManagerTrackingHoursProjectMembers { get; set; }

        public ProjectManagerTrackingHoursProjectViewModel()
        {
            OurProjectManagerTrackingHoursProjectMembers = new List<ProjectManagerTrackingHoursProjectMemberViewModel>();
        }
    }
}