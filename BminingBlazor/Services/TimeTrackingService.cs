using Models.TimeTracking;
using System;
using System.Collections.Generic;
using Data;
using Microsoft.Extensions.Configuration;

namespace BminingBlazor.Services
{
    public class TimeTrackingService : ITimeTrackingService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IConfiguration _configuration;
        public TimeTrackingService()
        {
        }

        public void AddUserTimeTracking(int userId, int projectId, DateTime timeTrackingDate, double trackedHours)
        {
            var trackingHourModel = new TimeTrackingModel
            {
                CreationDate = DateTime.UtcNow,
                ProjectId = projectId,
                TimeTrackingDate = timeTrackingDate,
                TimeTrackingStatus = TimeTrackingStatusEnum.WaitingForApproval,
                TrackedHours = trackedHours,
                UserId = userId
            };
        }
        public void ApproveUserTimeTracking(int timeTrackingId)
        {
            // Checkear si el usuario es el jefe de proyecto
            throw new NotImplementedException();
        }
        public void RejectUserTimeTracking(int timeTrackingId, string comment)
        {
            // Checkear si el usuario es el jefe de proyecto
            throw new NotImplementedException();
        }
        public List<TimeTrackingModel> GetUserTrackingModel(int userId, DateTime from, DateTime to)
        {
            return new List<TimeTrackingModel>();
        }
    }
}