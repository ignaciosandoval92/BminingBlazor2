using System;
using System.Collections.Generic;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class WeekDayUserTrackingHoursViewModel
    {
        public DateTime ItemTime { get; set; }
        public List<WeekDayUserTrackingHoursItemViewModel> WeekDayUserTrackingHourItems { get; set; }

        public WeekDayUserTrackingHoursViewModel()
        {
            WeekDayUserTrackingHourItems = new List<WeekDayUserTrackingHoursItemViewModel>();
        }
    }
}