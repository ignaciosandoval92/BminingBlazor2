using BminingBlazor.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class DashboardUserTrackingViewModel
    {
        private readonly ITimeTrackingService _timeTrackingService;
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int MyUserId { get; set; }

        public List<WeekDayUserTrackingHoursViewModel> WeekDayUserTrackingHours { get; set; }

        public DashboardUserTrackingViewModel(ITimeTrackingService timeTrackingService)
        {
            _timeTrackingService = timeTrackingService;
            WeekDayUserTrackingHours = new List<WeekDayUserTrackingHoursViewModel>();
        }

        public async Task SetStartDate(DateTime fromTime)
        {
            var numberOfDays = 7;
            WeekDayUserTrackingHours.Clear();
            FromTime = fromTime;
            ToTime = FromTime.AddDays(numberOfDays);

            var items = await _timeTrackingService.GetUserTrackingModel(MyUserId, FromTime, ToTime);
            
            for (int i = 0; i < numberOfDays; i++)
            {
                var currentDate = FromTime.AddDays(i);
                var weekDayUserTrackingHours = new WeekDayUserTrackingHoursViewModel
                {
                    ItemTime = currentDate,
                };
                WeekDayUserTrackingHours.Add(weekDayUserTrackingHours);

                var validItems = items.Where(model => model.MyTimeTrackingDate.Day == currentDate.Day &&
                                                      model.MyTimeTrackingDate.Month == currentDate.Month &&
                                                      model.MyTimeTrackingDate.Year == currentDate.Year);

                foreach (var timeTrackingViewModel in validItems)
                {
                    var weekDayUserTrackingHoursItem = new WeekDayUserTrackingHoursItemViewModel
                    {
                        MyCreationDate = timeTrackingViewModel.MyCreationDate,
                        MyProjectCode = timeTrackingViewModel.MyProjectCode,
                        MyProjectName = timeTrackingViewModel.MyProjectName,
                        MyTrackedHours = timeTrackingViewModel.MyTrackedHours,
                        MyProjectId = timeTrackingViewModel.MyProjectId,
                        MyTimeTimeTrackingStatus = timeTrackingViewModel.MyTimeTrackingStatus,
                        MyTimeTrackingDate = timeTrackingViewModel.MyTimeTrackingDate,
                        MyId = timeTrackingViewModel.MyId,
                    };
                    weekDayUserTrackingHours.OurWeekDayUserTrackingHourItems.Add(weekDayUserTrackingHoursItem);
                }
            }
        }
    }
}
