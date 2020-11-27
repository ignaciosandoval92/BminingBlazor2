using System.Collections.Generic;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class ProjectManagerTrackingHoursApprovalViewModel
    {
        public List<ProjectManagerTrackingHoursProjectViewModel> OurProjectManagerTrackingHoursProjects { get; set; }

        public ProjectManagerTrackingHoursApprovalViewModel()
        {
            OurProjectManagerTrackingHoursProjects = new List<ProjectManagerTrackingHoursProjectViewModel>();
        }
    }
}


