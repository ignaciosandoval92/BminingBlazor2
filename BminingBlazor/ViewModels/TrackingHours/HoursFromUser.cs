using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class HoursFromUsers
    {
        public int? MyManagerId { get; set; }
        public string MyManagerEmail { get; set; }
        public string MyManagerName { get; set; }
        public string MyManagerLastName { get; set; }
        public int MyUserId { get; set; }
        public string MyUserEmail { get; set; }
        public string MyUserName { get; set; }
        public string MyUserLastName { get; set; }
        public int MyProjectId { get; set; }
        public string MyProjectCode { get; set; }
        public string MyProjectName { get; set; }
        public int SendHours { get; set; }
        public int MyStatusHours { get; set; }     
        public double TrackedHours { get; set; }   
        public int Idhours { get; set; }
        public DateTime MyDate { get; set; }


    }
}