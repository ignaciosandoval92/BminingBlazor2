using System;

namespace Models.TimeTracking
{
    public class TimeTrackingModel
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime TimeTrackingDate { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public TimeTrackingStatusEnum TimeTrackingStatus { get; set; }
        public double TrackedHours { get; set; }
    }
}
