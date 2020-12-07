using System.Collections.Generic;

namespace BminingBlazor.ViewModels.ActivityRecord
{
    public class DashboardActivityRecordViewModel
    {
        public List<DashboardActivityRecordItemViewModel> OurDashboardActivityRecords { get; set; }

        public DashboardActivityRecordViewModel()
        {
            OurDashboardActivityRecords = new List<DashboardActivityRecordItemViewModel>();
        }
    }
}
