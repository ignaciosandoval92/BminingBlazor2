using System;
using BminingBlazor;
using BminingBlazor.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            _webHost = WebHost.CreateDefaultBuilder<Startup>(new string[0]).Build();
        }
        [TestMethod]
        public async Task ClientsFixture()
        {
            var clientDataService = (IClientDataService)_webHost.Services.GetService(typeof(IClientDataService));
            var clients = await clientDataService.ReadClients();
            var id = clients.First().ClientId;
            var client = await clientDataService.ReadClient(id);
            id = await clientDataService.CreateClient("Test");
            await clientDataService.DeleteClient(id);
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
            var timeTrackingViewModels = await timeTrackingService.GetUserTrackingModel(39, DateTime.MinValue, DateTime.MaxValue);
         
        }
    }
}
