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
            var id = clients.First().Id_Cliente;
            var client = await clientDataService.ReadClient(id);
            id = await clientDataService.CreateClient("Test");
            await clientDataService.DeleteClient(id);
        }

        [TestMethod]
        public async Task ReadProjectDummyFixture()
        {
            var dummyProjectDataService = (IDummyProjectDataService)_webHost
                .Services.GetService(typeof(IDummyProjectDataService));
            var projectViewModels =await dummyProjectDataService.GetProjectsOwnedById(39);

        }
    }
}
