using System;
using Models.ActivityRecord;

namespace BminingBlazor.ViewModels.ActivityRecord
{
    public class DashboardActivityRecordItemViewModel
    {
        public int MyId { get; set; }
        public string MyTitle { get; set; }
        public DateTime MyDate { get; set; }
        public int MyProjectId { get; set; }
        public int MyCreatorId { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }

    }
}