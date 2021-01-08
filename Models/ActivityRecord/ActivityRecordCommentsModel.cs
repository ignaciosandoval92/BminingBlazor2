using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ActivityRecord
{
    public class ActivityRecordCommentsModel
    {
        public int Id { get; set; }
        public int ActivityRecordId { get; set; }
        public string Commitment { get; set; }
        public string Responsible { get; set; }
        public DateTime CommitmentDate { get; set; }
        public int ActivityRecordStatus { get; set; }
        public bool IsVisibleInMainPanel { get; set; }
    }
}
