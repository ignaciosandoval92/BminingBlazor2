using Models.TimeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface ITimeTrackingService
    {
        void AddUserTimeTracking(int userId, int projectId, DateTime timeTrackingDate, double trackedHours);
        void ApproveUserTimeTracking(int timeTrackingId);
        Task<int> AddTimeTracking(TimeTrackingModel trackingHourModel);


        List<TimeTrackingModel> GetUserTrackingModel(int userId, DateTime from, DateTime to);
    }
}