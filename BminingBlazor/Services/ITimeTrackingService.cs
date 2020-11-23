using Models.TimeTracking;
using System;
using System.Collections.Generic;

namespace BminingBlazor.Services
{
    public interface ITimeTrackingService
    {
        void AddUserTimeTracking(int userId, int projectId, DateTime timeTrackingDate, double trackedHours);
        void ApproveUserTimeTracking(int timeTrackingId);
        
        List<TimeTrackingModel> GetUserTrackingModel(int userId, DateTime from, DateTime to);
    }
}