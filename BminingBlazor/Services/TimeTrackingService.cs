using BminingBlazor.Utility;
using BminingBlazor.ViewModels.TrackingHours;
using Data;
using Microsoft.Extensions.Configuration;
using Models.TimeTracking;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<int> AddUserTimeTracking(int userId, int projectId, DateTime timeTrackingDate,
            double trackedHours)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var id = await queryFactory.Query(TableConstants.TimeTrackingTable).InsertGetIdAsync<int>(new Dictionary<string, object>
            {
                {TimeTrackingConstants.UserId,userId},
                {TimeTrackingConstants.CreationDate,DateTime.UtcNow},
                {TimeTrackingConstants.ProjectId,projectId},
                {TimeTrackingConstants.TimeTrackingDate,timeTrackingDate},
                {TimeTrackingConstants.TrackedHours,trackedHours}
            });
            return id;
        }


        public async Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByProjectManager(int projectManagerId)
        {
            var projectManagerTrackingHoursApproval = new ProjectManagerTrackingHoursApprovalViewModel();

            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var memberQuery = queryFactory.Query(TableConstants.UserTable);

            var mainQuery = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                    $"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}")
                .Where($"{ProjectConstants.ProjectManagerId}", projectManagerId)
                .Where($"{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval)
                .Include(TableConstants.UserTable, memberQuery, $"{UserConstants.UserId}",
                                                                          $"{MemberConstants.UserId}")
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectManagerId},{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode}}}");

            var items = (await mainQuery.GetAsync()).Cast<IDictionary<string, object>>().ToList();


            foreach (var grouping in items.GroupBy(objects => objects[ProjectConstants.ProjectId]))
            {
                if (!grouping.Any()) continue;
                var projectTemplate = grouping.First();


                var projectManagerTrackingHoursProject = new ProjectManagerTrackingHoursProjectViewModel
                {
                    MyProjectId = (int)projectTemplate[ProjectConstants.ProjectId],
                    MyProjectName = (string)projectTemplate[ProjectConstants.ProjectName],
                    MyProjectCode = (string)projectTemplate[ProjectConstants.ProjectCode],
                };

                projectManagerTrackingHoursApproval.OurProjectManagerTrackingHoursProjects.Add(projectManagerTrackingHoursProject);

                foreach (var scenario in grouping)
                {
                    var user = (IDictionary<string, object>)scenario[TableConstants.UserTable];
                    projectManagerTrackingHoursProject.OurProjectManagerTrackingHoursProjectMembers.Add(new ProjectManagerTrackingHoursProjectMemberViewModel
                    {
                        MyHoursLoaded = (double)scenario[TimeTrackingConstants.TrackedHours],
                        DateOfHours = (DateTime)scenario[TimeTrackingConstants.TimeTrackingDate],
                        MyMemberName = (string)user[UserConstants.Name] + " " + user[UserConstants.PaternalLastName],
                        TimeTrackingHoursId = (int)scenario[TimeTrackingConstants.TimeTrackingId],
                        MyMemberEmail = (string)user[UserConstants.EmailBmining],
                        MyUserId = (int)user[UserConstants.UserId]
                    });
                }
            }
            return projectManagerTrackingHoursApproval;
        }

        public async Task<List<TimeTrackingViewModel>> GetUserTrackingModel(int userId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.UserId, userId)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(ProjectConstants.ProjectManager, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode}}}");


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
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],
                    MyProjectManager = ViewModelConverter.GetUserViewModel(projectManagerUser)
                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }

        public async Task RemoveTimeTrackingHour(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(TableConstants.TimeTrackingTable).Where(TimeTrackingConstants.TimeTrackingId, id)
                .DeleteAsync();
        }

        public async Task EditStatusTimeTracking(int id, TimeTrackingStatusEnum waitingForApproval, double hours)
        {
            if (hours < 0)
                throw new Exception($"Invalid hours {hours}");
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(TableConstants.TimeTrackingTable).Where(TimeTrackingConstants.TimeTrackingId, id)
                .UpdateAsync(new Dictionary<string, object>
                {
                    {TimeTrackingConstants.TimeTrackingStatusId, (int) waitingForApproval},
                    {TimeTrackingConstants.TrackedHours,hours}
                });
        }

        public async Task ApproveUserTimeTracking(int timeTrackingId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(TableConstants.TimeTrackingTable).
                              Where(TimeTrackingConstants.TimeTrackingId,timeTrackingId).
                              UpdateAsync(new Dictionary<string, object>
            {
                {TimeTrackingConstants.TimeTrackingStatusId, (int) TimeTrackingStatusEnum.Approved}
            });
        }
        public async Task RejectUserTimeTracking(int timeTrackingId, string comment)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(TableConstants.TimeTrackingTable).
                Where(TimeTrackingConstants.TimeTrackingId, timeTrackingId).
                UpdateAsync(new Dictionary<string, object>
                {
                    {TimeTrackingConstants.TimeTrackingStatusId, (int) TimeTrackingStatusEnum.Rejected}
                });
        }
    }
}