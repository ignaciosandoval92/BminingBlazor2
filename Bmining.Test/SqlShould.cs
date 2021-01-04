using BminingBlazor;
using BminingBlazor.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bmining.Test
{
    [TestClass]
    public class SqlShould
    {
        private readonly IWebHost _webHost;

        public SqlShould()
        {
            _webHost = WebHost.CreateDefaultBuilder<Startup>(new string[0]).UseEnvironment("Development").Build();
        }
        [TestMethod]
        public async Task ClientsFixture()
        {
            var clientDataService = (IClientDataService)_webHost.Services.GetService(typeof(IClientDataService));
            var projectDataService = (IProjectDataService)_webHost.Services.GetService(typeof(IProjectDataService));
            var clients = await clientDataService.ReadClients();
            var id = clients.First().MyId;
            var client = await clientDataService.ReadClient(id);
            id = await clientDataService.CreateClient("Test");
            await clientDataService.DeleteClient(id);
            await projectDataService.ReadProjects();
        }

        [TestMethod]
        public async Task ReadProjectDummyFixture()
        {
            var dummyProjectDataService = (IDummyProjectDataService)_webHost
                .Services.GetService(typeof(IDummyProjectDataService));
            var projectViewModels = await dummyProjectDataService.GetProjectsOwnedById(39);
        }
        [TestMethod]
        public async Task ReadTimeTrackingHoursFixture()
        {
            var timeTrackingService = (ITimeTrackingService)_webHost.Services.GetService(typeof(ITimeTrackingService));
            var timeTrackingViewModels = await timeTrackingService.GetUserTrackingModels(39, DateTime.MinValue, DateTime.MaxValue);
        }
        [TestMethod]
        public async Task AddTimeTrackingHoursFixture()
        {
            var timeTrackingService = (ITimeTrackingService)_webHost.Services.GetService(typeof(ITimeTrackingService));
            var dateTime = DateTime.Now;
            var timeTrackingViewModels = await timeTrackingService.AddUserTimeTracking(39, 32, dateTime, 7.5);
        }

        [TestMethod]
        public async Task ReadProjectsWhereBelongsUserIdFixture()
        {
            var dummyProjectDataService = (IDummyProjectDataService)_webHost.Services.GetService(typeof(IDummyProjectDataService));
            var projectResumeViewModels = await dummyProjectDataService.GetProjectWhereBelongsUserId(39);
        }
        [TestMethod]
        public async Task GetPendingTimeTrackingHoursByProjectManagerFixture()
        {
            var timeTrackingService = (ITimeTrackingService)_webHost.Services.GetService(typeof(ITimeTrackingService));
            var items = await timeTrackingService.GetPendingTimeTrackingHoursByProjectManager(39);
        }
        [TestMethod]
        public async Task GetActivityRecordFixture()
        {
            var activityRecordService = (IActivityRecordService)_webHost.Services.GetService(typeof(IActivityRecordService));
            var activityRecordViewModel = await activityRecordService.GetActivityRecord(5);
            activityRecordViewModel.MyDurationHours = 4;
            await activityRecordService.EditActivityRecord(activityRecordViewModel);
        }

        [TestMethod]
        public async Task GetAllActivityRecordsFixture()
        {
            var activityRecordService = (IActivityRecordService)_webHost.Services.GetService(typeof(IActivityRecordService));
            var activityRecordViewModel = await activityRecordService.GetActivityRecords();
        }

        [TestMethod]
        public async Task GetAllActivityRecordsByUserFixture()
        {
            var activityRecordService = (IActivityRecordService)_webHost.Services.GetService(typeof(IActivityRecordService));
            var activityRecordViewModel = await activityRecordService.GetActivityRecords(39);
        }
        [TestMethod]
        public async Task ReadProjectsWhereUserBelongs()
        {
            var projectDataService = (IProjectDataService)_webHost.Services.GetService(typeof(IProjectDataService));
            var simpleProjectViewModels = await projectDataService.ReadProjectWhereUserBelongs(39);
        }
    }
}
