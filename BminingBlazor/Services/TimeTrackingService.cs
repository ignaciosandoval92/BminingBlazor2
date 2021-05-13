using BminingBlazor.Resources;
using BminingBlazor.Utility;
using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.TrackingHours;
using Data;
using Microsoft.Extensions.Configuration;
using Models.TimeTracking;
using SendGrid;
using SendGrid.Helpers.Mail;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Data;
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

        public async Task<int> AddUserTimeTrackingOrdinary(int userId, int projectId, DateTime timeTrackingDate,
            double trackedHours)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var id = await queryFactory.Query(TableConstants.TimeTrackingTable).InsertGetIdAsync<int>(new Dictionary<string, object>
            {
                {TimeTrackingConstants.UserId,userId},
                {TimeTrackingConstants.CreationDate,DateTime.UtcNow},
                {TimeTrackingConstants.ProjectId,projectId},
                {TimeTrackingConstants.TimeTrackingDate,timeTrackingDate},
                {TimeTrackingConstants.TrackedHours,trackedHours},
                {TimeTrackingConstants.TypeTrackingHours,0 }
            });
            return id;
        }
        public async Task<int> AddUserTimeTrackingExtraordinary(int userId, int projectId, DateTime timeTrackingDate,
            double trackedHours)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var id = await queryFactory.Query(TableConstants.TimeTrackingTable).InsertGetIdAsync<int>(new Dictionary<string, object>
            {
                {TimeTrackingConstants.UserId,userId},
                {TimeTrackingConstants.CreationDate,DateTime.UtcNow},
                {TimeTrackingConstants.ProjectId,projectId},
                {TimeTrackingConstants.TimeTrackingDate,timeTrackingDate},
                {TimeTrackingConstants.TrackedHours,trackedHours},
                {TimeTrackingConstants.TypeTrackingHours,1 }
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
                .Where($"{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
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
        public async Task<ProjectManagerTrackingHoursApprovalViewModel> GetPendingTimeTrackingHoursByAdmin(int adminId)
        {
            var projectManagerTrackingHoursApproval = new ProjectManagerTrackingHoursApprovalViewModel();

            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var memberQuery = queryFactory.Query(TableConstants.UserTable);

            var mainQuery = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                    $"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}")
                .Join(TableConstants.AdminAuditHoursTable, $"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.ProjectId}", $"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}")
                .Where($"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.UserId}", adminId)
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

        public async Task<List<ReportViewModel>> GetUserTrackingModels(int userId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.UserId, userId)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}", (int)ProjectStatusEnum.Active)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}");



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var timeTrackingViewModels = new List<ReportViewModel>();
            foreach (var item in items)
            {
                var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                var projectManagerUser = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
                var timeTrackingViewModel = new ReportViewModel
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
                    MyProjectManager = ViewModelConverter.GetUserViewModel(projectManagerUser),
                    MyUser = ViewModelConverter.GetUserViewModel(user)
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
                              Where(TimeTrackingConstants.TimeTrackingId, timeTrackingId).
                              UpdateAsync(new Dictionary<string, object>
            {
                {TimeTrackingConstants.TimeTrackingStatusId, (int) TimeTrackingStatusEnum.Approved}
            });
        }


        public async Task<ReportViewModel> GetTimeTrackingId(int timeTrackingId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);



            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.TimeTrackingId, timeTrackingId)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                   $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}");

            var item = (await query.GetAsync()).Cast<IDictionary<string, object>>().First();

            var projectManagerUser = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
            var user = (IDictionary<string, object>)item[TableConstants.UserTable];

            var timeTrackingViewModel = new ReportViewModel
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
                MyProjectManager = ViewModelConverter.GetUserViewModel(projectManagerUser),
                MyUser = ViewModelConverter.GetUserViewModel(user)
            };
            return timeTrackingViewModel;
        }

        public async Task RejectUserTimeTracking(int timeTrackingId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var timeTracking = await GetTimeTrackingId(timeTrackingId);
            await queryFactory.Query(TableConstants.TimeTrackingTable).
                Where(TimeTrackingConstants.TimeTrackingId, timeTrackingId).
                UpdateAsync(new Dictionary<string, object>
                {
                    {TimeTrackingConstants.TimeTrackingStatusId, (int) TimeTrackingStatusEnum.Rejected},
                    {TimeTrackingConstants.SendHours,(int)TimeTrackingSendHoursEnum.NotSend }
                });

        }
        public async Task<List<ProjectResumeViewModel>> ChargedProjectOrdinary(int userId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.UserId, userId)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}", (int)ProjectStatusEnum.Active)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}").Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Ordinary)
                        .GroupBy(ProjectConstants.ProjectId);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var timeTrackingViewModels = new List<ProjectResumeViewModel>();
            foreach (var item in items)
            {

                var timeTrackingViewModel = new ProjectResumeViewModel
                {

                    MyProjectId = (int)item[TimeTrackingConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],

                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }
        public async Task<List<ProjectResumeViewModel>> ChargedProjectExtraordinary(int userId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.UserId, userId)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}", (int)ProjectStatusEnum.Active)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}").Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Extraordinary)
                        .GroupBy(ProjectConstants.ProjectId);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var timeTrackingViewModels = new List<ProjectResumeViewModel>();
            foreach (var item in items)
            {

                var timeTrackingViewModel = new ProjectResumeViewModel
                {

                    MyProjectId = (int)item[TimeTrackingConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],

                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }
        public async Task RemoveWeekTrackingHoursFromProject(int idProject, int idUser, DateTime from, DateTime to, int TypeHours)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.ProjectId, idProject)
                .Where(TimeTrackingConstants.UserId, idUser)
                .Where(TimeTrackingConstants.TypeTrackingHours, TypeHours)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .DeleteAsync();
        }
        public async Task<ProjectTrackingWeekViewModel> ReadProjectWeekOrdinary(int idProject, int idUser, DateTime startDate, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            DateTime from = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.UserId, idUser)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}", idProject)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}").Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Ordinary);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();
            double sat = new double();
            double sun = new double();
            double mon = new double();
            double tue = new double();
            double wed = new double();
            double thu = new double();
            double fri = new double();
            int idSat = new int();
            int idSun = new int();
            int idMon = new int();
            int idTue = new int();
            int idWed = new int();
            int idThu = new int();
            int idFri = new int();
            string projectName = "";
            string projectCode = "";
            int SendHours = new int();
            int StatusHours = new int();
            string UserEmail = "";
            string UserName = "";
            string UserLastName = "";
            string ManagerEmail = "";
            string ManagerName = "";
            string ManagerLastName = "";


            foreach (var item in items)
            {

                if (from.Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    sat = (double)item[TimeTrackingConstants.TrackedHours];
                    idSat = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(1).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    sun = (double)item[TimeTrackingConstants.TrackedHours];
                    idSun = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(2).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    mon = (double)item[TimeTrackingConstants.TrackedHours];
                    idMon = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(3).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    tue = (double)item[TimeTrackingConstants.TrackedHours];
                    idTue = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(4).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    wed = (double)item[TimeTrackingConstants.TrackedHours];
                    idWed = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(5).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    thu = (double)item[TimeTrackingConstants.TrackedHours];
                    idThu = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(6).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    fri = (double)item[TimeTrackingConstants.TrackedHours];
                    idFri = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                var projectManagerUser = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
                projectName = (string)item[ProjectConstants.ProjectName];
                projectCode = (string)item[ProjectConstants.ProjectCode];
                SendHours = (int)item[TimeTrackingConstants.SendHours];
                StatusHours = (int)item[TimeTrackingConstants.TimeTrackingStatusId];
                UserEmail = (string)user[UserConstants.EmailBmining];
                UserName = (string)user[UserConstants.Name];
                UserLastName = (string)user[UserConstants.PaternalLastName];
                ManagerEmail = (string)projectManagerUser[UserConstants.EmailBmining];
                ManagerName = (string)projectManagerUser[UserConstants.Name];
                ManagerLastName = (string)projectManagerUser[UserConstants.PaternalLastName];
            };
            var timeTrackingViewModel = new ProjectTrackingWeekViewModel
            {

                MyProjectId = idProject,
                MyProjectName = projectName,
                MyProjectCode = projectCode,
                StartWeek = from,
                EndWeek = to,
                TrackedHoursSat = sat,
                TrackedHoursSun = sun,
                TrackedHoursMon = mon,
                TrackedHoursTue = tue,
                TrackedHoursWed = wed,
                TrackedHoursThu = thu,
                TrackedHoursFri = fri,
                IdSat = idSat,
                IdSun = idSun,
                IdMon = idMon,
                IdTue = idTue,
                IdWed = idWed,
                IdThu = idThu,
                IdFri = idFri,
                SendHours = SendHours,
                MyUserEmail = UserEmail,
                MyUserName = UserName,
                MyUserLastName = UserLastName,
                MyManagerEmail = ManagerEmail,
                MyManagerName = ManagerName,
                MyManagerLastName = ManagerLastName,
                MyStatusHours=StatusHours
            };
            return timeTrackingViewModel;
        }
        public async Task<ProjectTrackingWeekViewModel> ReadProjectWeekExtraordinary(int idProject, int idUser, DateTime startDate, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            DateTime from = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.UserId, idUser)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}", idProject)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}").Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Extraordinary);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();
            double sat = new double();
            double sun = new double();
            double mon = new double();
            double tue = new double();
            double wed = new double();
            double thu = new double();
            double fri = new double();
            int idSat = new int();
            int idSun = new int();
            int idMon = new int();
            int idTue = new int();
            int idWed = new int();
            int idThu = new int();
            int idFri = new int();
            string projectName = "";
            string projectCode = "";
            int SendHours = new int();
            int StatusHours = new int();



            foreach (var item in items)
            {

                if (from.Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    sat = (double)item[TimeTrackingConstants.TrackedHours];
                    idSat = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(1).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    sun = (double)item[TimeTrackingConstants.TrackedHours];
                    idSun = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(2).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    mon = (double)item[TimeTrackingConstants.TrackedHours];
                    idMon = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(3).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    tue = (double)item[TimeTrackingConstants.TrackedHours];
                    idTue = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(4).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    wed = (double)item[TimeTrackingConstants.TrackedHours];
                    idWed = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(5).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    thu = (double)item[TimeTrackingConstants.TrackedHours];
                    idThu = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                if (from.AddDays(6).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                {
                    fri = (double)item[TimeTrackingConstants.TrackedHours];
                    idFri = (int)item[TimeTrackingConstants.TimeTrackingId];
                }
                var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                var projectManagerUser = (IDictionary<string, object>)item[ProjectConstants.ProjectManager];
                projectName = (string)item[ProjectConstants.ProjectName];
                projectCode = (string)item[ProjectConstants.ProjectCode];
                SendHours = (int)item[TimeTrackingConstants.SendHours];
                StatusHours = (int)item[TimeTrackingConstants.TimeTrackingStatusId];

            };
            var timeTrackingViewModel = new ProjectTrackingWeekViewModel
            {

                MyProjectId = idProject,
                MyProjectName = projectName,
                MyProjectCode = projectCode,
                StartWeek = from,
                EndWeek = to,
                TrackedHoursSat = sat,
                TrackedHoursSun = sun,
                TrackedHoursMon = mon,
                TrackedHoursTue = tue,
                TrackedHoursWed = wed,
                TrackedHoursThu = thu,
                TrackedHoursFri = fri,
                IdSat = idSat,
                IdSun = idSun,
                IdMon = idMon,
                IdTue = idTue,
                IdWed = idWed,
                IdThu = idThu,
                IdFri = idFri,
                SendHours = SendHours,
                MyStatusHours=StatusHours
            };
            return timeTrackingViewModel;
        }
        public async Task SendTrackedHours(int id)
        {

            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            await queryFactory.Query(TableConstants.TimeTrackingTable).Where(TimeTrackingConstants.TimeTrackingId, id)
                .UpdateAsync(new Dictionary<string, object>
                {
                    {TimeTrackingConstants.SendHours, (int)TimeTrackingSendHoursEnum.Send},
                    {TimeTrackingConstants.TimeTrackingStatusId,(int)TimeTrackingStatusEnum.WaitingForApproval}

                });
        }
        public async Task<List<ProjectTrackingWeekViewModel>> GetPendingWeekExtraordinaryFromManager(int idProject, int idManager, DateTime startDate, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            DateTime from = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}", idProject)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Extraordinary)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval)
                        .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectManagerId}", idManager);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();
            List<ProjectTrackingWeekViewModel> timeTrackingsViewModel = new List<ProjectTrackingWeekViewModel>();
            List<int> Users = new List<int>();

            foreach (var item in items)
            {
                if (Users.Contains((int)item[UserConstants.UserId]))
                {

                }
                else
                {
                    Users.Add((int)item[UserConstants.UserId]);
                }
            }
            foreach (var idu in Users)
            {
                double sat = new double();
                double sun = new double();
                double mon = new double();
                double tue = new double();
                double wed = new double();
                double thu = new double();
                double fri = new double();
                int idSat = new int();
                int idSun = new int();
                int idMon = new int();
                int idTue = new int();
                int idWed = new int();
                int idThu = new int();
                int idFri = new int();
                string projectName = "";
                string projectCode = "";
                int sendHours = new int();
                int? userId = new int();
                string userEmail = "";



                foreach (var item in items)
                {
                    if ((int)item[UserConstants.UserId] == idu)
                    {
                        if (from.Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sat = (double)item[TimeTrackingConstants.TrackedHours];
                            idSat = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(1).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sun = (double)item[TimeTrackingConstants.TrackedHours];
                            idSun = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(2).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            mon = (double)item[TimeTrackingConstants.TrackedHours];
                            idMon = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(3).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            tue = (double)item[TimeTrackingConstants.TrackedHours];
                            idTue = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(4).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            wed = (double)item[TimeTrackingConstants.TrackedHours];
                            idWed = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(5).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            thu = (double)item[TimeTrackingConstants.TrackedHours];
                            idThu = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(6).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            fri = (double)item[TimeTrackingConstants.TrackedHours];
                            idFri = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        projectName = (string)item[ProjectConstants.ProjectName];
                        projectCode = (string)item[ProjectConstants.ProjectCode];
                        sendHours = (int)item[TimeTrackingConstants.SendHours];
                        userId = (int)item[UserConstants.UserId];
                        var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                        userEmail = (string)user[UserConstants.EmailBmining];

                    }
                };
                var timeTrackingViewModel = new ProjectTrackingWeekViewModel
                {

                    MyProjectId = idProject,
                    MyProjectName = projectName,
                    MyProjectCode = projectCode,
                    StartWeek = from,
                    EndWeek = to,
                    TrackedHoursSat = sat,
                    TrackedHoursSun = sun,
                    TrackedHoursMon = mon,
                    TrackedHoursTue = tue,
                    TrackedHoursWed = wed,
                    TrackedHoursThu = thu,
                    TrackedHoursFri = fri,
                    IdSat = idSat,
                    IdSun = idSun,
                    IdMon = idMon,
                    IdTue = idTue,
                    IdWed = idWed,
                    IdThu = idThu,
                    IdFri = idFri,
                    SendHours = sendHours,
                    MyUserId = userId,
                    MyUserEmail = userEmail
                };

                timeTrackingsViewModel.Add(timeTrackingViewModel);
            }
            return timeTrackingsViewModel;
        }
        public async Task<List<ProjectResumeViewModel>> GetProjectExtraordinaryByManager(int managerId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}", (int)ProjectStatusEnum.Active)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Extraordinary)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                .Where($"{ProjectConstants.ProjectManagerId}", managerId)
                        .GroupBy(ProjectConstants.ProjectId);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var timeTrackingViewModels = new List<ProjectResumeViewModel>();
            foreach (var item in items)
            {

                var timeTrackingViewModel = new ProjectResumeViewModel
                {

                    MyProjectId = (int)item[TimeTrackingConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],

                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }
        public async Task<List<ProjectResumeViewModel>> GetProjectOrdinaryByManager(int managerId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}", (int)ProjectStatusEnum.Active)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Ordinary)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval)
                .Where($"{ProjectConstants.ProjectManagerId}", managerId)
                        .GroupBy(ProjectConstants.ProjectId);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var timeTrackingViewModels = new List<ProjectResumeViewModel>();
            foreach (var item in items)
            {

                var timeTrackingViewModel = new ProjectResumeViewModel
                {

                    MyProjectId = (int)item[TimeTrackingConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],

                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }
        public async Task<List<ProjectTrackingWeekViewModel>> GetPendingWeekOrdinaryFromManager(int idProject, int idManager, DateTime startDate, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            DateTime from = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}", idProject)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Ordinary)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval)
                        .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectManagerId}", idManager);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();
            List<ProjectTrackingWeekViewModel> timeTrackingsViewModel = new List<ProjectTrackingWeekViewModel>();
            List<int> Users = new List<int>();

            foreach (var item in items)
            {
                if (Users.Contains((int)item[UserConstants.UserId]))
                {

                }
                else
                {
                    Users.Add((int)item[UserConstants.UserId]);
                }
            }
            foreach (var idu in Users)
            {
                double sat = new double();
                double sun = new double();
                double mon = new double();
                double tue = new double();
                double wed = new double();
                double thu = new double();
                double fri = new double();
                int idSat = new int();
                int idSun = new int();
                int idMon = new int();
                int idTue = new int();
                int idWed = new int();
                int idThu = new int();
                int idFri = new int();
                string projectName = "";
                string projectCode = "";
                int sendHours = new int();
                int? userId = new int();
                string userEmail = "";



                foreach (var item in items)
                {
                    if ((int)item[UserConstants.UserId] == idu)
                    {
                        if (from.Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sat = (double)item[TimeTrackingConstants.TrackedHours];
                            idSat = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(1).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sun = (double)item[TimeTrackingConstants.TrackedHours];
                            idSun = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(2).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            mon = (double)item[TimeTrackingConstants.TrackedHours];
                            idMon = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(3).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            tue = (double)item[TimeTrackingConstants.TrackedHours];
                            idTue = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(4).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            wed = (double)item[TimeTrackingConstants.TrackedHours];
                            idWed = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(5).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            thu = (double)item[TimeTrackingConstants.TrackedHours];
                            idThu = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(6).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            fri = (double)item[TimeTrackingConstants.TrackedHours];
                            idFri = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        projectName = (string)item[ProjectConstants.ProjectName];
                        projectCode = (string)item[ProjectConstants.ProjectCode];
                        sendHours = (int)item[TimeTrackingConstants.SendHours];
                        userId = (int)item[UserConstants.UserId];
                        var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                        userEmail = (string)user[UserConstants.EmailBmining];

                    }
                };
                var timeTrackingViewModel = new ProjectTrackingWeekViewModel
                {

                    MyProjectId = idProject,
                    MyProjectName = projectName,
                    MyProjectCode = projectCode,
                    StartWeek = from,
                    EndWeek = to,
                    TrackedHoursSat = sat,
                    TrackedHoursSun = sun,
                    TrackedHoursMon = mon,
                    TrackedHoursTue = tue,
                    TrackedHoursWed = wed,
                    TrackedHoursThu = thu,
                    TrackedHoursFri = fri,
                    IdSat = idSat,
                    IdSun = idSun,
                    IdMon = idMon,
                    IdTue = idTue,
                    IdWed = idWed,
                    IdThu = idThu,
                    IdFri = idFri,
                    SendHours = sendHours,
                    MyUserId = userId,
                    MyUserEmail = userEmail
                };

                timeTrackingsViewModel.Add(timeTrackingViewModel);
            }
            return timeTrackingsViewModel;
        }
        public async Task<List<ProjectTrackingWeekViewModel>> GetPendingWeekOrdinaryFromAdmin(int idProject, int idAdmin, DateTime startDate, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            DateTime from = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}", idProject)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Join(TableConstants.AdminAuditHoursTable, $"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.ProjectId}", $"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}")
                .Where($"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.UserId}", idAdmin)
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Ordinary)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval);
                        



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();
            List<ProjectTrackingWeekViewModel> timeTrackingsViewModel = new List<ProjectTrackingWeekViewModel>();
            List<int> Users = new List<int>();

            foreach (var item in items)
            {
                if (Users.Contains((int)item[UserConstants.UserId]))
                {

                }
                else
                {
                    Users.Add((int)item[UserConstants.UserId]);
                }
            }
            foreach (var idu in Users)
            {
                double sat = new double();
                double sun = new double();
                double mon = new double();
                double tue = new double();
                double wed = new double();
                double thu = new double();
                double fri = new double();
                int idSat = new int();
                int idSun = new int();
                int idMon = new int();
                int idTue = new int();
                int idWed = new int();
                int idThu = new int();
                int idFri = new int();
                string projectName = "";
                string projectCode = "";
                int sendHours = new int();
                int? userId = new int();
                string userEmail = "";



                foreach (var item in items)
                {
                    if ((int)item[UserConstants.UserId] == idu)
                    {
                        if (from.Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sat = (double)item[TimeTrackingConstants.TrackedHours];
                            idSat = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(1).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sun = (double)item[TimeTrackingConstants.TrackedHours];
                            idSun = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(2).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            mon = (double)item[TimeTrackingConstants.TrackedHours];
                            idMon = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(3).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            tue = (double)item[TimeTrackingConstants.TrackedHours];
                            idTue = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(4).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            wed = (double)item[TimeTrackingConstants.TrackedHours];
                            idWed = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(5).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            thu = (double)item[TimeTrackingConstants.TrackedHours];
                            idThu = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(6).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            fri = (double)item[TimeTrackingConstants.TrackedHours];
                            idFri = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        projectName = (string)item[ProjectConstants.ProjectName];
                        projectCode = (string)item[ProjectConstants.ProjectCode];
                        sendHours = (int)item[TimeTrackingConstants.SendHours];
                        userId = (int)item[UserConstants.UserId];
                        var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                        userEmail = (string)user[UserConstants.EmailBmining];

                    }
                };
                var timeTrackingViewModel = new ProjectTrackingWeekViewModel
                {

                    MyProjectId = idProject,
                    MyProjectName = projectName,
                    MyProjectCode = projectCode,
                    StartWeek = from,
                    EndWeek = to,
                    TrackedHoursSat = sat,
                    TrackedHoursSun = sun,
                    TrackedHoursMon = mon,
                    TrackedHoursTue = tue,
                    TrackedHoursWed = wed,
                    TrackedHoursThu = thu,
                    TrackedHoursFri = fri,
                    IdSat = idSat,
                    IdSun = idSun,
                    IdMon = idMon,
                    IdTue = idTue,
                    IdWed = idWed,
                    IdThu = idThu,
                    IdFri = idFri,
                    SendHours = sendHours,
                    MyUserId = userId,
                    MyUserEmail = userEmail
                };

                timeTrackingsViewModel.Add(timeTrackingViewModel);
            }
            return timeTrackingsViewModel;
        }
        public async Task<List<ProjectResumeViewModel>> GetProjectOrdinaryByAdmin(int AdminId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}", (int)ProjectStatusEnum.Active)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                  .Join(TableConstants.AdminAuditHoursTable, $"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.ProjectId}", $"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}")
                .Where($"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.UserId}", AdminId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Ordinary)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval)                
                        .GroupBy(ProjectConstants.ProjectId);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var timeTrackingViewModels = new List<ProjectResumeViewModel>();
            foreach (var item in items)
            {

                var timeTrackingViewModel = new ProjectResumeViewModel
                {

                    MyProjectId = (int)item[TimeTrackingConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],

                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }
        public async Task<List<ProjectResumeViewModel>> GetProjectExtraordinaryByAdmin(int AdminId, DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.StatusId}", (int)ProjectStatusEnum.Active)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                  .Join(TableConstants.AdminAuditHoursTable, $"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.ProjectId}", $"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}")
                .Where($"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.UserId}", AdminId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Extraordinary)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval)
                        .GroupBy(ProjectConstants.ProjectId);



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var timeTrackingViewModels = new List<ProjectResumeViewModel>();
            foreach (var item in items)
            {

                var timeTrackingViewModel = new ProjectResumeViewModel
                {

                    MyProjectId = (int)item[TimeTrackingConstants.ProjectId],
                    MyProjectName = (string)item[ProjectConstants.ProjectName],
                    MyProjectCode = (string)item[ProjectConstants.ProjectCode],

                };
                timeTrackingViewModels.Add(timeTrackingViewModel);
            }
            return timeTrackingViewModels;
        }
        public async Task<List<ProjectTrackingWeekViewModel>> GetPendingWeekExtraordinaryFromAdmin(int idProject, int idAdmin, DateTime startDate, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            DateTime from = new DateTime(startDate.Year, startDate.Month, startDate.Day);
            var userQuery = queryFactory.Query(TableConstants.UserTable);
            var managerQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}", idProject)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Join(TableConstants.AdminAuditHoursTable, $"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.ProjectId}", $"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}")
                .Where($"{TableConstants.AdminAuditHoursTable}.{AdminAuditHoursConstants.UserId}", idAdmin)
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Include(ProjectConstants.ProjectManager, managerQuery, ProjectConstants.ProjectManagerId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode},{ProjectConstants.ProjectManagerId}}}")
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TypeTrackingHours}", (int)TimeTrackingTypeEnum.Extraordinary)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.SendHours}", (int)TimeTrackingSendHoursEnum.Send)
                        .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.TimeTrackingStatusId}", (int)TimeTrackingStatusEnum.WaitingForApproval);




            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();
            List<ProjectTrackingWeekViewModel> timeTrackingsViewModel = new List<ProjectTrackingWeekViewModel>();
            List<int> Users = new List<int>();

            foreach (var item in items)
            {
                if (Users.Contains((int)item[UserConstants.UserId]))
                {

                }
                else
                {
                    Users.Add((int)item[UserConstants.UserId]);
                }
            }
            foreach (var idu in Users)
            {
                double sat = new double();
                double sun = new double();
                double mon = new double();
                double tue = new double();
                double wed = new double();
                double thu = new double();
                double fri = new double();
                int idSat = new int();
                int idSun = new int();
                int idMon = new int();
                int idTue = new int();
                int idWed = new int();
                int idThu = new int();
                int idFri = new int();
                string projectName = "";
                string projectCode = "";
                int sendHours = new int();
                int? userId = new int();
                string userEmail = "";



                foreach (var item in items)
                {
                    if ((int)item[UserConstants.UserId] == idu)
                    {
                        if (from.Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sat = (double)item[TimeTrackingConstants.TrackedHours];
                            idSat = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(1).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            sun = (double)item[TimeTrackingConstants.TrackedHours];
                            idSun = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(2).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            mon = (double)item[TimeTrackingConstants.TrackedHours];
                            idMon = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(3).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            tue = (double)item[TimeTrackingConstants.TrackedHours];
                            idTue = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(4).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            wed = (double)item[TimeTrackingConstants.TrackedHours];
                            idWed = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(5).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            thu = (double)item[TimeTrackingConstants.TrackedHours];
                            idThu = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        if (from.AddDays(6).Day == ((DateTime)item[TimeTrackingConstants.TimeTrackingDate]).Day)
                        {
                            fri = (double)item[TimeTrackingConstants.TrackedHours];
                            idFri = (int)item[TimeTrackingConstants.TimeTrackingId];
                        }
                        projectName = (string)item[ProjectConstants.ProjectName];
                        projectCode = (string)item[ProjectConstants.ProjectCode];
                        sendHours = (int)item[TimeTrackingConstants.SendHours];
                        userId = (int)item[UserConstants.UserId];
                        var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                        userEmail = (string)user[UserConstants.EmailBmining];

                    }
                };
                var timeTrackingViewModel = new ProjectTrackingWeekViewModel
                {

                    MyProjectId = idProject,
                    MyProjectName = projectName,
                    MyProjectCode = projectCode,
                    StartWeek = from,
                    EndWeek = to,
                    TrackedHoursSat = sat,
                    TrackedHoursSun = sun,
                    TrackedHoursMon = mon,
                    TrackedHoursTue = tue,
                    TrackedHoursWed = wed,
                    TrackedHoursThu = thu,
                    TrackedHoursFri = fri,
                    IdSat = idSat,
                    IdSun = idSun,
                    IdMon = idMon,
                    IdTue = idTue,
                    IdWed = idWed,
                    IdThu = idThu,
                    IdFri = idFri,
                    SendHours = sendHours,
                    MyUserId = userId,
                    MyUserEmail = userEmail
                };

                timeTrackingsViewModel.Add(timeTrackingViewModel);
            }
            return timeTrackingsViewModel;
        }
    }
}