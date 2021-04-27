﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.ViewModels.TrackingHours
{
    public class ProjectTrackingWeekViewModel
    {
        public int MyProjectId { get; set; }
        public string MyProjectCode { get; set; }
        public string MyProjectName { get; set; }
        public int SendHours { get; set; }
        public DateTime StartWeek { get; set; }
        public DateTime EndWeek { get; set; }
        public double TrackedHoursSat { get; set; }
        public double TrackedHoursSun { get; set; }
        public double TrackedHoursMon { get; set; }
        public double TrackedHoursTue { get; set; }
        public double TrackedHoursWed { get; set; }
        public double TrackedHoursThu { get; set; }
        public double TrackedHoursFri { get; set; }
        public int IdSat { get; set; }
        public int IdSun { get; set; }
        public int IdMon { get; set; }
        public int IdTue { get; set; }
        public int IdWed { get; set; }
        public int IdThu { get; set; }
        public int IdFri { get; set; }


    }
}
