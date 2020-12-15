using BminingBlazor.ViewModels.User;
using Models.TimeTracking;
using System;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class TimeTrackingViewModel
    {
        public int MyId { get; set; }
        public DateTime MyCreationDate { get; set; }
        public DateTime MyTimeTrackingDate { get; set; }
        public int MyProjectId { get; set; }
        public string MyProjectCode { get; set; }
        public string MyProjectName { get; set; }
        public UserViewModel MyProjectManager { get; set; }
        public UserViewModel MyUser {get;set;}
        public int MyUserId { get; set; }
        public TimeTrackingStatusEnum MyTimeTrackingStatus { get; set; }
        public double MyTrackedHours { get; set; }
    }
}