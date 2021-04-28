using Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BminingBlazor.ViewModels.TrackingHours;
using BminingBlazor.ViewModels.Projects;

namespace BminingBlazor.Services
{
    public interface ITimeTrackingService
    {
        Task<int> AddUserTimeTrackingOrdinary(int userId, int projectId, DateTime timeTrackingDate, double trackedHours);
        Task<int> AddUserTimeTrackingExtraordinary(int userId, int projectId, DateTime timeTrackingDate, double trackedHours);
        Task ApproveUserTimeTracking(int timeTrackingId);
        Task RejectUserTimeTracking(int timeTrackingId);
        Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByProjectManager(int projectManagerId);
        Task<List<ReportViewModel>> GetUserTrackingModels(int userId, DateTime from, DateTime to);
        Task RemoveTimeTrackingHour(int id);
        Task EditStatusTimeTracking(int id, TimeTrackingStatusEnum waitingForApproval, double hours);
        Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByAdmin(int adminId);
        Task<List<ProjectResumeViewModel>> ChargedProjectOrdinary(int userId, DateTime from, DateTime to);
        Task<List<ProjectResumeViewModel>> ChargedProjectExtraordinary(int userId, DateTime from, DateTime to);
        Task RemoveWeekTrackingHoursFromProject(int idProject, int idUser, DateTime from, DateTime to,int TypeHours);
        Task<ProjectTrackingWeekViewModel> ReadProjectWeekOrdinary(int idProject, int idUser, DateTime startDate, DateTime to);
        Task<ProjectTrackingWeekViewModel> ReadProjectWeekExtraordinary(int idProject, int idUser, DateTime startDate, DateTime to);
        Task SendTrackedHours(int id);
        Task<List<ProjectTrackingWeekViewModel>> GetPendingWeekExtraordinaryFromManager(int idProject, int idManager, DateTime startDate, DateTime to);
        Task<List<ProjectTrackingWeekViewModel>> GetPendingWeekOrdinaryFromManager(int idProject, int idManager, DateTime startDate, DateTime to);
        Task<List<ProjectResumeViewModel>> GetProjectExtraordinaryByManager(int managerId, DateTime from, DateTime to);
        Task<List<ProjectResumeViewModel>> GetProjectOrdinaryByManager(int managerId, DateTime from, DateTime to);
    }
}