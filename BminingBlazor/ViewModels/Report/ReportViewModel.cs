using BminingBlazor.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.Report
{
    public class ReportViewModel : UserViewModel 
    {
        public double MyTrackedHours { get; set; }
        public DateTime MyDateTracked { get; set; }
        public string MyCodProject { get; set; }
        public string MyNameProject { get; set; }
    }
}
