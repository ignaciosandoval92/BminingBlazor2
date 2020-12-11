using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using BminingBlazor.ViewModels.Projects;
using Models.Project;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace BminingBlazor.Utility
{
    public static class Extensions
    {     
        public static async Task<string> GetEmail( this AuthenticationStateProvider AuthenticationStateProvider)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User.Identity.Name;
            return user;
        }

        public static ProjectViewModel GetBaseProjectViewModel(this ProjectModel projectModel)
        {
            return new ProjectViewModel
            {
                MyProjectName = projectModel.ProjectName,
                MyProjectCode = projectModel.CodProject,
                MyClientId = projectModel.ClientId,
                OurPayments = new List<PaymentViewModel>(),
                MyId = projectModel.ProjectId,
                MyStartDate = projectModel.Fecha_Inicio, // TODO: Revisar Los UTC
                MyEndDate = projectModel.Fecha_Fin
            };
        }

    }    
}
