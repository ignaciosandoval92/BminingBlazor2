using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class DashboardUserTrackingViewModel
    {
        public DateTime FromTime { get; set; }
        public ObservableCollection<DashboardUserTrackingItemViewModel> DashboardUserTrackingModels { get; set; }
        public DashboardUserTrackingViewModel()
        {
            DashboardUserTrackingModels = new ObservableCollection<DashboardUserTrackingItemViewModel>();
        }

        public void SetWeekUserModels(int week, int year)
        {
            FromTime = new DateTime(year, 1, 1);
            FromTime = FromTime.AddDays(7 * week);
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
