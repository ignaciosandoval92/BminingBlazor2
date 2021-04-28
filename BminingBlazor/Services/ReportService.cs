using BminingBlazor.Utility;
using BminingBlazor.ViewModels.Projects;
using BminingBlazor.ViewModels.Report;
using Data;
using Microsoft.Extensions.Configuration;
using Models.TimeTracking;
using Models.User;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public class ReportService : IReportService
    {
        private readonly IDataAccess _dataAccess;
        private readonly string _connectionString;
        public ReportService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _connectionString = configuration.GetConnectionString("default");
        }

        public async Task<List<ReportViewModel>> GetUserReport(int userId, DateTime from, DateTime to,int projectId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);           

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.UserId}", userId)
                .Where(TimeTrackingConstants.TimeTrackingStatusId,(int)TimeTrackingStatusEnum.Approved)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}",projectId)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Join(TableConstants.UserTable,$"{TableConstants.UserTable}.{UserConstants.UserId}",
                $"{TableConstants.TimeTrackingTable}.{UserConstants.UserId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)                
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode}}}",
                        $"{TableConstants.UserTable}.{{{UserConstants.Name},{UserConstants.PaternalLastName},{UserConstants.WorkArea}}}");



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var report = new List<ReportViewModel>();
            foreach (var item in items)
            {
                var user = (IDictionary<string, object>)item[TableConstants.UserTable];                
                var reportViewModel = new ReportViewModel
                {
                    MyCodProject=(string)item[ProjectConstants.ProjectCode],
                    MyNameProject=(string)item[ProjectConstants.ProjectName],
                    MyName = (string)item[UserConstants.Name],
                    MyPaternalSurname = (string)item[UserConstants.PaternalLastName],
                    MyTrackedHours = (double)item[TimeTrackingConstants.TrackedHours],
                    MyDateTracked = (DateTime)item[TimeTrackingConstants.TimeTrackingDate],
                    MyWorkArea = (WorkAreaModelEnum)item[UserConstants.WorkArea]
                };
                report.Add(reportViewModel);
            }
            return report;
        }

        public async Task<List<ReportViewModel>> GetProjectReport(DateTime from, DateTime to, int projectId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)                
                .Where(TimeTrackingConstants.TimeTrackingStatusId, (int)TimeTrackingStatusEnum.Approved)
                .Where($"{TableConstants.TimeTrackingTable}.{TimeTrackingConstants.ProjectId}", projectId)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Join(TableConstants.UserTable, $"{TableConstants.UserTable}.{UserConstants.UserId}",
                $"{TableConstants.TimeTrackingTable}.{UserConstants.UserId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode}}}",
                        $"{TableConstants.UserTable}.{{{UserConstants.Name},{UserConstants.PaternalLastName},{UserConstants.WorkArea}}}");



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var report = new List<ReportViewModel>();
            foreach (var item in items)
            {
                var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                var reportViewModel = new ReportViewModel
                {
                    MyCodProject = (string)item[ProjectConstants.ProjectCode],
                    MyNameProject = (string)item[ProjectConstants.ProjectName],
                    MyName = (string)item[UserConstants.Name],
                    MyPaternalSurname = (string)item[UserConstants.PaternalLastName],
                    MyTrackedHours = (double)item[TimeTrackingConstants.TrackedHours],
                    MyDateTracked = (DateTime)item[TimeTrackingConstants.TimeTrackingDate],
                    MyWorkArea = (WorkAreaModelEnum)item[UserConstants.WorkArea]
                };
                report.Add(reportViewModel);
            }
            return report;
        }
        public async Task<List<ReportViewModel>> GetHoursChargedReport( DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)                
                .Where(TimeTrackingConstants.TimeTrackingStatusId, (int)TimeTrackingStatusEnum.WaitingForApproval)               
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Join(TableConstants.UserTable, $"{TableConstants.UserTable}.{UserConstants.UserId}",
                $"{TableConstants.TimeTrackingTable}.{UserConstants.UserId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode}}}",
                        $"{TableConstants.UserTable}.{{{UserConstants.Name},{UserConstants.PaternalLastName},{UserConstants.WorkArea}}}");



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var report = new List<ReportViewModel>();
            foreach (var item in items)
            {
                var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                var reportViewModel = new ReportViewModel
                {
                    MyCodProject = (string)item[ProjectConstants.ProjectCode],
                    MyNameProject = (string)item[ProjectConstants.ProjectName],
                    MyName = (string)item[UserConstants.Name],
                    MyPaternalSurname = (string)item[UserConstants.PaternalLastName],
                    MyTrackedHours = (double)item[TimeTrackingConstants.TrackedHours],
                    MyDateTracked = (DateTime)item[TimeTrackingConstants.TimeTrackingDate],
                    MyWorkArea=(WorkAreaModelEnum)item[UserConstants.WorkArea]
                };
                report.Add(reportViewModel);
            }
            return report;
        }
        public async Task<List<ReportViewModel>> GetHoursApprovedReport(DateTime from, DateTime to)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);

            var userQuery = queryFactory.Query(TableConstants.UserTable);

            var query = queryFactory.Query(TableConstants.TimeTrackingTable)
                .Where(TimeTrackingConstants.TimeTrackingStatusId, (int)TimeTrackingStatusEnum.Approved)
                .WhereBetween(TimeTrackingConstants.TimeTrackingDate, from, to)
                .Join(TableConstants.ProjectTable, $"{TableConstants.ProjectTable}.{ProjectConstants.ProjectId}",
                                                  $"{TableConstants.TimeTrackingTable}.{ProjectConstants.ProjectId}")
                .Join(TableConstants.UserTable, $"{TableConstants.UserTable}.{UserConstants.UserId}",
                $"{TableConstants.TimeTrackingTable}.{UserConstants.UserId}")
                .Include(TableConstants.UserTable, userQuery, TimeTrackingConstants.UserId, UserConstants.UserId)
                .Select($"{TableConstants.TimeTrackingTable}.{{*}}",
                        $"{TableConstants.ProjectTable}.{{{ProjectConstants.ProjectName},{ProjectConstants.ProjectCode}}}",
                        $"{TableConstants.UserTable}.{{{UserConstants.Name},{UserConstants.PaternalLastName},{UserConstants.WorkArea}}}");



            var items = (await query.GetAsync()).Cast<IDictionary<string, object>>().ToList();

            var report = new List<ReportViewModel>();
            foreach (var item in items)
            {
                var user = (IDictionary<string, object>)item[TableConstants.UserTable];
                var reportViewModel = new ReportViewModel
                {
                    MyCodProject = (string)item[ProjectConstants.ProjectCode],
                    MyNameProject = (string)item[ProjectConstants.ProjectName],
                    MyName = (string)item[UserConstants.Name],
                    MyPaternalSurname = (string)item[UserConstants.PaternalLastName],
                    MyTrackedHours = (double)item[TimeTrackingConstants.TrackedHours],
                    MyDateTracked = (DateTime)item[TimeTrackingConstants.TimeTrackingDate],
                    MyWorkArea = (WorkAreaModelEnum)item[UserConstants.WorkArea]
                };
                report.Add(reportViewModel);
            }
            return report;
        }
    }
}
