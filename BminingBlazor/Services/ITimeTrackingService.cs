using Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.TrackingHours;

namespace BminingBlazor.Services
{
    public interface ITimeTrackingService
    {
        Task AddUserTimeTracking(int userId, int projectId, DateTime timeTrackingDate, double trackedHours);
        Task ApproveUserTimeTracking(int timeTrackingId);
        Task RejectUserTimeTracking(int timeTrackingId,string reason);
        Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByProjectManager(int projectManagerId);
        Task<List<TimeTrackingViewModel>> GetUserTrackingModel(int userId, DateTime from, DateTime to);
    }
}