using System;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class ProjectManagerTrackingHoursProjectMemberViewModel
    {
        public int MyUserId { get; set; }
        public string MyMemberName { get; set; }
        public string MyMemberEmail { get; set; }
        public double MyHoursLoaded { get; set; }
        public DateTime DateOfHours { get; set; }
        public int TimeTrackingHoursId { get; set; }
    }
}