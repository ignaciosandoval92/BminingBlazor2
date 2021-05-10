﻿using BminingBlazor.ViewModels.User;
using Models.TimeTracking;
using Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.Report
{
    public class ReportViewModel : UserViewModel 
    {
        public int MyManagerId { get; set; }
        public double MyTrackedHours { get; set; }
        public DateTime MyDateTracked { get; set; }
        public string MyCodProject { get; set; }
        public string MyNameProject { get; set; }
        public WorkAreaModelEnum MyWorkArea { get; set; }
        public TimeTrackingTypeEnum MyTypeHours { get; set; }

    }
}
