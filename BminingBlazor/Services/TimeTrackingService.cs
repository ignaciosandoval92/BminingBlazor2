using BminingBlazor.ViewModels.TrackingHours;
using Data;
using Microsoft.Extensions.Configuration;
using Models.TimeTracking;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BminingBlazor.Utility;

namespace BminingBlazor.Services
{
    public class TimeTrackingService : ITimeTrackingService
    {
        private readonly IDataAccess _dataAccess;
        private readonly string _connectionString;
        public TimeTrackingService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _connectionString = configuration.GetConnectionString("default");

        }

        public async Task AddUserTimeTracking(int userId, int projectId, DateTime timeTrackingDate, double trackedHours)
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
            var sql = "INSERT INTO TimeTracking (TimeTracking.CreationDate,TimeTracking.TimeTrackingDate,TimeTracking.Id_Proyecto,TimeTracking.Id_Usuario,TimeTracking.Id_TimeTrackingStatus,TimeTracking.TrackedHours) " +
                      "VALUES (@CreationDate,@TimeTrackingDate,@ProjectId,@UserId,@TimeTrackingStatus,@TrackedHours);";
            await _dataAccess.SaveData(sql, trackingHourModel,  _connectionString);
        }


        public Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByProjectManager(int projectManagerId)
        {
            var projectManagerTrackingHoursApproval = new ProjectManagerTrackingHoursApprovalViewModel();

            // Project 4
            var projectManagerTrackingHoursProject4 = new ProjectManagerTrackingHoursProjectViewModel
            {
                MyProjectId = 4,
                MyProjectName = "P CODE 4",
                MyProjectCode = "Proyecto número 4",
            };

            projectManagerTrackingHoursProject4.OurProjectManagerTrackingHoursProjectMembers.Add(new ProjectManagerTrackingHoursProjectMemberViewModel
            {
                MyHoursLoaded = 4.5,
                MyMemberEmail = "test_user1@bmining.cl",
                MyMemberName = "Test User 1",
                MyUserId = 4,
                DateOfHours = DateTime.Now.AddDays(2),
                TimeTrackingHoursId = 856,
            });

            projectManagerTrackingHoursProject4.OurProjectManagerTrackingHoursProjectMembers.Add(new ProjectManagerTrackingHoursProjectMemberViewModel
            {
                MyHoursLoaded = 9,
                MyMemberEmail = "test_user2@bmining.cl",
                MyMemberName = "Test User 2",
                MyUserId = 5,
                DateOfHours = DateTime.Now.AddDays(-1),
                TimeTrackingHoursId = 2,
            });

            projectManagerTrackingHoursProject4.OurProjectManagerTrackingHoursProjectMembers.Add(new ProjectManagerTrackingHoursProjectMemberViewModel
            {
                MyHoursLoaded = 9,
                MyMemberEmail = "test_user3@bmining.cl",
                MyMemberName = "Test User 3",
                MyUserId = 6,
                DateOfHours = DateTime.Now.AddDays(3),
                TimeTrackingHoursId = 16,
            });

            // Project 5
            var projectManagerTrackingHoursProject2 = new ProjectManagerTrackingHoursProjectViewModel
            {
                MyProjectId = 2,
                MyProjectName = "P CODE 2",
                MyProjectCode = "Proyecto número 2",
            };

            projectManagerTrackingHoursProject2.OurProjectManagerTrackingHoursProjectMembers.Add(new ProjectManagerTrackingHoursProjectMemberViewModel
            {
                MyHoursLoaded = 3.1,
                MyMemberEmail = "test_user4@bmining.cl",
                MyMemberName = "Test User 4",
                MyUserId = 4,
                DateOfHours = DateTime.Now.AddDays(4),
                TimeTrackingHoursId = 28,
            });

            projectManagerTrackingHoursProject2.OurProjectManagerTrackingHoursProjectMembers.Add(new ProjectManagerTrackingHoursProjectMemberViewModel
            {
                MyHoursLoaded = 6.7,
                MyMemberEmail = "test_user5@bmining.cl",
                MyMemberName = "Test User 5",
                MyUserId = 5,
                DateOfHours = DateTime.Now.AddDays(6),
                TimeTrackingHoursId = 37,
            });

            projectManagerTrackingHoursApproval.OurProjectManagerTrackingHoursProjects.Add(projectManagerTrackingHoursProject4);
            projectManagerTrackingHoursApproval.OurProjectManagerTrackingHoursProjects.Add(projectManagerTrackingHoursProject2);
            return Task.Run(() => projectManagerTrackingHoursApproval);
        }

        public async Task<List<TimeTrackingViewModel>> GetUserTrackingModel(int userId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.UserId, userId)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate,from,to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                   $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(ProjectConstants.ProjectManager, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.CodProject}}}");


            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

             var timeTrackingViewModels = new List<TimeTrackingViewModel>();
            foreach (var item in items)
            {
                var projectManagerUser = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
                var timeTrackingViewModel = new TimeTrackingViewModel
                {
                    MyId = (int)item[TimeTrackingConstants.TimeTrackingId],
                    MyProjectId = (int)item[TimeTrackingConstants.ProjectId],
                    MyUserId = (int)item[TimeTrackingConstants.UserId],
                    MyCreationDate = (DateTime)item[TimeTrackingConstants.CreationDate],
                    MyTimeTrackingDate = (DateTime)item[TimeTrackingConstants.TimeTrackingDate],
                    MyTimeTrackingStatus = (TimeTrackingStatusEnum)item[TimeTrackingConstants.TimeTrackingStatusId],
                    MyTrackedHours = (double)item[TimeTrackingConstants.TrackedHours],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyProjectCode = (string)item[ProjectConstants.CodProject],
                    MyProjectManager = ViewModelConverter.GetUserViewModel(projectManagerUser)
                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }

        public Task ApproveUserTimeTracking(int timeTrackingId)
        {
            return Task.Run(() => { });
        }
        public Task RejectUserTimeTracking(int timeTrackingId, string comment)
        {
            return Task.Run(() => { });
        }
    }
}