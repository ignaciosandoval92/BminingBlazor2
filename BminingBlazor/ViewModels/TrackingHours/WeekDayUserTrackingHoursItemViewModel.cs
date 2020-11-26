using System;
using Models.TimeTracking;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class WeekDayUserTrackingHoursItemViewModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime TimeTrackingDate { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
      

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
                        MyTrackingStatusClass = "list-group-item-dark";
                        break;
                    case TimeTrackingStatusEnum.Approved:
                        MyTrackingStatusClass = "list-group-item-success";
                        break;
                    case TimeTrackingStatusEnum.Rejected:
                        MyTrackingStatusClass = "list-group-item-danger";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }


            }
        }


        public string MyTrackingStatusClass { get; set; }
        public double TrackedHours { get; set; }
    }
}