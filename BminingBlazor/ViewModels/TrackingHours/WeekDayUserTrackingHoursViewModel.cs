using System;
using System.Collections.Generic;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class WeekDayUserTrackingHoursViewModel
    {
        public DateTime ItemTime { get; set; }
        public List<WeekDayUserTrackingHoursItemViewModel> OurWeekDayUserTrackingHourItems { get; set; }

        public WeekDayUserTrackingHoursViewModel()
        {
            OurWeekDayUserTrackingHourItems = new List<WeekDayUserTrackingHoursItemViewModel>();
        }
    }
}