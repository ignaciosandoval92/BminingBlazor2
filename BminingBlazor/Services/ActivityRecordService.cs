﻿using BminingBlazor.ViewModels.ActivityRecord;
using Data;
using Microsoft.Extensions.Configuration;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public class ActivityRecordService : IActivityRecordService
    {
        private readonly IDataAccess _dataAccess;
        private readonly string _connectionString;
        public ActivityRecordService(IDataAccess dataAccess, IConfiguration configuration)
        {
            _dataAccess = dataAccess;
            _connectionString = configuration.GetConnectionString("default");
        }
        public async Task<int> CreateActivityRecordAsync(string title, int creatorId)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var query = queryFactory.Query(TableConstants.ActivityRecordTable);
            var index = await query.InsertGetIdAsync<int>(new Dictionary<string, object>
            {
                {ActivityRecordConstants.Title,title},
                {ActivityRecordConstants.CreatorId,creatorId},
                {ActivityRecordConstants.Date ,DateTime.UtcNow},
            });
            return index;
        }

        public async Task<ActivityRecordViewModel> GetActivityRecord(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var activityQuery = queryFactory.Query(TableConstants.ActivityRecordTable);
            var membersQuery = queryFactory.Query(TableConstants.ActivityRecordMemberTable);
            var commitmentQuery = queryFactory.Query(TableConstants.ActivityRecordCommitmentTable);

            var item = (await activityQuery.Where(ActivityRecordConstants.Id, id)
                                          .IncludeMany(TableConstants.ActivityRecordMemberTable, membersQuery,
                                                       $"{ActivityRecordMemberConstants.ActivityRecordId}",
                                                       $"{ActivityRecordConstants.Id}")
                                          .IncludeMany(TableConstants.ActivityRecordCommitmentTable, commitmentQuery,
                                                       $"{ActivityRecordCommitmentConstants.ActivityRecordId}",
                                                       $"{ActivityRecordConstants.Id}").GetAsync())
                                          .Cast<IDictionary<string, object>>().ToList().First();

            var activityRecord = new ActivityRecordViewModel
            {
                MyId = (int)item[ActivityRecordConstants.Id],
                MyDate = (DateTime)item[ActivityRecordConstants.Date],
                MyDurationHours = (double?)item[ActivityRecordConstants.DurationHours],
                MyNotes = (string)item[ActivityRecordConstants.Notes],
                MyPlace = (string)item[ActivityRecordConstants.Place],
                MyTitle = (string)item[ActivityRecordConstants.Title],
                MySecurityReflection = (string)item[ActivityRecordConstants.SecurityReflection]
            };
            var members = (IEnumerable<IDictionary<string, object>>)item[TableConstants.ActivityRecordMemberTable];
            foreach (var member in members)
            {
                var activityRecordMember = new ActivityRecordMemberViewModel
                {
                    MyId = (int)member[ActivityRecordMemberConstants.Id],
                    MyEmail = (string)member[ActivityRecordMemberConstants.Email],
                    MyRut = (string)member[ActivityRecordMemberConstants.Rut],
                    MyName = (string)member[ActivityRecordMemberConstants.Name],
                    MySurname = (string)member[ActivityRecordMemberConstants.Surname],
                    MyEnterprise = (string)member[ActivityRecordMemberConstants.Enterprise],
                    MyBminingId = (int?)member[ActivityRecordMemberConstants.BminingId],
                    IsBminingMember = (ulong)member[ActivityRecordMemberConstants.IsBminingMember] == 1,
                };
                activityRecord.OurMembers.Add(activityRecordMember);
            }
            var commitments = (IEnumerable<IDictionary<string, object>>)item[TableConstants.ActivityRecordCommitmentTable];
            foreach (var commitment in commitments)
            {
                var activityRecordCommitment = new ActivityRecordCommitmentViewModel
                {
                    MyId = (int)commitment[ActivityRecordCommitmentConstants.Id],
                    MyResponsible = (string)commitment[ActivityRecordCommitmentConstants.Responsible],
                    MyCommitment = (string)commitment[ActivityRecordCommitmentConstants.Commitment],
                    MyCommitmentDate = (DateTime)commitment[ActivityRecordCommitmentConstants.CommitmentDate]
                };
                activityRecord.OurCommitments.Add(activityRecordCommitment);
            }
            return activityRecord;
        }

        public async Task<DashboardActivityRecordViewModel> GetActivityRecords()
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var items = (await queryFactory.Query(TableConstants.ActivityRecordTable).GetAsync()).Cast<IDictionary<string, object>>().ToList();
            var dashboardActivityRecord = new DashboardActivityRecordViewModel();
            foreach (var item in items)
            {
                var dashboardActivityRecordItem = new DashboardActivityRecordItemViewModel()
                {
                    MyId = (int)item[ActivityRecordConstants.Id],
                    MyDate = (DateTime)item[ActivityRecordConstants.Date],
                    MyTitle = (string)item[ActivityRecordConstants.Title]
                };
                dashboardActivityRecord.OurDashboardActivityRecords.Add(dashboardActivityRecordItem);
            }
            return dashboardActivityRecord;
        }

        public async Task DeleteActivityRecord(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var activityQuery = queryFactory.Query(TableConstants.ActivityRecordTable);
            var membersQuery = queryFactory.Query(TableConstants.ActivityRecordMemberTable);
            var commitmentQuery = queryFactory.Query(TableConstants.ActivityRecordCommitmentTable);


            var activityRecord = await GetActivityRecord(id);
            await activityQuery.Where(ActivityRecordConstants.Id, id).DeleteAsync();

            foreach (var commitment in activityRecord.OurCommitments)
            {
                await commitmentQuery.Where(ActivityRecordCommitmentConstants.Id,commitment.MyId).DeleteAsync();
            }
            foreach (var member in activityRecord.OurMembers)
            {
                await membersQuery.Where(ActivityRecordMemberConstants.Id, member.MyId).DeleteAsync();
            }
        }

        public async Task EditActivityRecord(ActivityRecordViewModel activityRecord)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var activityQuery = queryFactory.Query(TableConstants.ActivityRecordTable);

            var item = activityQuery.Where(ActivityRecordConstants.Id, activityRecord.MyId);
            await item.UpdateAsync(new Dictionary<string, object>
            {
                {ActivityRecordConstants.Date,activityRecord.MyDate},
                {ActivityRecordConstants.DurationHours,activityRecord.MyDurationHours},
                {ActivityRecordConstants.Notes,activityRecord.MyNotes},
                {ActivityRecordConstants.Place,activityRecord.MyPlace},
                {ActivityRecordConstants.Title,activityRecord.MyTitle},
                {ActivityRecordConstants.SecurityReflection,activityRecord.MySecurityReflection}
            });
        }

        public async Task AddActivityRecordMember(ActivityRecordMemberViewModel activityRecordMember)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var activityQuery = queryFactory.Query(TableConstants.ActivityRecordMemberTable);

            var id = await activityQuery.InsertGetIdAsync<int>(new Dictionary<string, object>
            {
                {ActivityRecordMemberConstants.Name,activityRecordMember.MyName},
                {ActivityRecordMemberConstants.Surname,activityRecordMember.MySurname},
                {ActivityRecordMemberConstants.Rut,activityRecordMember.MyRut},
                {ActivityRecordMemberConstants.Email,activityRecordMember.MyEmail},
                {ActivityRecordMemberConstants.ActivityRecordId,activityRecordMember.MyActivityRecordId},
                {ActivityRecordMemberConstants.IsBminingMember,activityRecordMember.IsBminingMember},
                {ActivityRecordMemberConstants.BminingId,activityRecordMember.MyBminingId},
                {ActivityRecordMemberConstants.Enterprise,activityRecordMember.MyEnterprise}
            });
            activityRecordMember.MyId = id;
        }

        public async Task DeleteActivityRecordMember(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var activityQuery = queryFactory.Query(TableConstants.ActivityRecordMemberTable);
            var numberOfRows = await activityQuery.Where(ActivityRecordMemberConstants.Id, id).DeleteAsync();
        }

        public async Task AddActivityRecordCommitment(ActivityRecordCommitmentViewModel activityRecordCommitment)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var activityQuery = queryFactory.Query(TableConstants.ActivityRecordCommitmentTable);
            var id = await activityQuery.InsertGetIdAsync<int>(new Dictionary<string, object>
            {
                {ActivityRecordCommitmentConstants.ActivityRecordId, activityRecordCommitment.MyActivityRecordId},
                {ActivityRecordCommitmentConstants.Commitment,activityRecordCommitment.MyCommitment},
                {ActivityRecordCommitmentConstants.CommitmentDate,activityRecordCommitment.MyCommitmentDate},
                {ActivityRecordCommitmentConstants.Responsible,activityRecordCommitment.MyResponsible}
            });
            activityRecordCommitment.MyId = id;
        }

        public async Task DeleteActivityRecordCommitment(int id)
        {
            var queryFactory = _dataAccess.GetQueryFactory(_connectionString);
            var activityQuery = queryFactory.Query(TableConstants.ActivityRecordCommitmentTable);
            var numberOfRows = await activityQuery.Where(ActivityRecordCommitmentConstants.Id, id).DeleteAsync();
        }
    }
}