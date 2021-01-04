using Models.ActivityRecord;
using System;

namespace BminingBlazor.ViewModels.ActivityRecord
{
    public class ActivityRecordCommitmentViewModel
    {
        public int MyId { get; set; }
        public int MyActivityRecordId { get; set; }
        public string MyCommitment { get; set; }
        public string MyResponsible { get; set; }
        public DateTime MyCommitmentDate { get; set; }
        public ActivityRecordStatusEnum MyStatus { get; set; }
    }
}