﻿using Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.TrackingHours;
using BminingBlazor.ViewModels.Projects;

namespace BminingBlazor.Services
{
    public interface ITimeTrackingService
    {
        Task<int> AddUserTimeTracking(int userId, int projectId, DateTime timeTrackingDate, double trackedHours);
        Task ApproveUserTimeTracking(int timeTrackingId);
        Task RejectUserTimeTracking(int timeTrackingId,string reason);
        Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByProjectManager(int projectManagerId);
        Task<List<ReportViewModel>> GetUserTrackingModels(int userId, DateTime from, DateTime to);
        Task RemoveTimeTrackingHour(int id);
        Task EditStatusTimeTracking(int id, TimeTrackingStatusEnum waitingForApproval, double hours);
        Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByAdmin(int adminId);
        Task<List<ProjectResumeViewModel>> ChargedProject(int userId, DateTime from, DateTime to);
    }
}