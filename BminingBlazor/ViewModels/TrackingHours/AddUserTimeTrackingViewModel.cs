using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BminingBlazor.ViewModels.Projects;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class AddUserTimeTrackingViewModel
    {
        public int UserId { get; }
        public List<ProjectResumeViewModel> OurAvailableProjects { get; set; }
        public ProjectResumeViewModel SelectedProject { get; set; }
        public int? MySelectedProjectId { get; set; }
        [Required]
        public double MyHours { get; set; }
        public DateTime MyTrackingHourDate { get; set; }
        public AddUserTimeTrackingViewModel(int userId)
        {
            UserId = userId;
            OurAvailableProjects = new List<ProjectResumeViewModel>();
        }
    }
}
