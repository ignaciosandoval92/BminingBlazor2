using Models.TimeTracking;
using System;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class DashboardUserTrackingItemViewModel
    {
        public DateTime TimeTrackingDate { get; }
        public double TrackedHours { get; }
        public string ProjectCode { get; }
        public int ProjectId { get; set; }
        public TimeTrackingStatusEnum TimeTrackingStatus { get; set; }
        public DashboardUserTrackingItemViewModel(TimeTrackingModel timeTrackingModel)
        {
            TimeTrackingDate = timeTrackingModel.TimeTrackingDate;
            TrackedHours = timeTrackingModel.TrackedHours;
            TimeTrackingStatus = timeTrackingModel.TimeTrackingStatus;
        }
    }
}