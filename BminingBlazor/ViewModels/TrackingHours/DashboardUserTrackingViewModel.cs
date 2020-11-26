using Models.TimeTracking;
using System;
using System.Collections.Generic;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class DashboardUserTrackingViewModel
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }

        public List<WeekDayUserTrackingHoursViewModel> WeekDayUserTrackingHours { get; set; }
        public DashboardUserTrackingViewModel()
        {
            WeekDayUserTrackingHours = new List<WeekDayUserTrackingHoursViewModel>
            {

            };


        }

        public void SetStartDate(DateTime fromTime)
        {
            WeekDayUserTrackingHours.Clear();

            FromTime = fromTime;
            ToTime = FromTime.AddDays(7);

            for (int i = 0; i < 7; i++)
            {
                var weekDayUserTrackingHours = new WeekDayUserTrackingHoursViewModel
                {
                    ItemTime = FromTime.AddDays(i)
                };
                WeekDayUserTrackingHours.Add(weekDayUserTrackingHours);

                for (int j = 0; j <= i; j++)
                {
                    var weekDayUserTrackingHoursItem = new WeekDayUserTrackingHoursItemViewModel();
                    weekDayUserTrackingHoursItem.CreationDate = DateTime.Now;
                    weekDayUserTrackingHoursItem.ProjectCode = $"Code Project 14 {i}-{j}";
                    weekDayUserTrackingHoursItem.ProjectName = $"Name {i}-{j}";
                    weekDayUserTrackingHoursItem.TrackedHours = Math.Round(i * j * 0.78, 2);

                    var value = i * j;

                    if (value % 2 == 0)
                        weekDayUserTrackingHoursItem.MyTimeTimeTrackingStatus = TimeTrackingStatusEnum.Approved;
                    else if (value % 3 == 0)
                        weekDayUserTrackingHoursItem.MyTimeTimeTrackingStatus = TimeTrackingStatusEnum.Rejected;
                    else
                        weekDayUserTrackingHoursItem.MyTimeTimeTrackingStatus = TimeTrackingStatusEnum.WaitingForApproval;

                    weekDayUserTrackingHoursItem.TimeTrackingDate = FromTime.AddDays(i);
                    weekDayUserTrackingHours
                        .WeekDayUserTrackingHourItems
                            .Add(weekDayUserTrackingHoursItem);
                }
            }
        }

        public List<DateTime> GetDates()
        {
            var dates = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                dates.Add(FromTime.AddDays(i));
            }
            return dates;
        }
    }
}
