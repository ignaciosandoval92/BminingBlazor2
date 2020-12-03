using System;
using Models.TimeTracking;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class WeekDayUserTrackingHoursItemViewModel
    {
        public int MyId { get; set; }
        public DateTime MyCreationDate { get; set; }
        public DateTime MyTimeTrackingDate { get; set; }
        public int MyProjectId { get; set; }
        public string MyProjectName { get; set; }
        public string MyProjectCode { get; set; }
      
        private TimeTrackingStatusEnum _myTimeTimeTrackingStatus;
        public TimeTrackingStatusEnum MyTimeTimeTrackingStatus
        {
            get { return _myTimeTimeTrackingStatus; }
            set
            {
                _myTimeTimeTrackingStatus = value;
                switch (value)
                {
                    case TimeTrackingStatusEnum.WaitingForApproval:
                        MyTrackingStatusClass = "bd-bmining-info";
                        break;
                    case TimeTrackingStatusEnum.Approved:
                        MyTrackingStatusClass = "bd-bmining-success";
                        break;
                    case TimeTrackingStatusEnum.Rejected:
                        MyTrackingStatusClass = "bd-bmining-error";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }


            }
        }
        public string MyTrackingStatusClass { get; set; }
        public double MyTrackedHours { get; set; }
    }
}