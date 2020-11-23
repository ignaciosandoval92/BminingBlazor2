using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class AddUserTimeTrackingViewModel
    {
        public int MyUserId { get; }
        public List<ProjectDetailsViewModel> OurAvailableProjects { get; set; }
        public int? MySelectedProjectId { get; set; }
        [Required]
        public double MyHours { get; set; }
        public DateTime MyTrackingHourDate { get; set; }
        public AddUserTimeTrackingViewModel(int myUserId)
        {
            MyUserId = myUserId;
            OurAvailableProjects = new List<ProjectDetailsViewModel>();
        }
    }
}
