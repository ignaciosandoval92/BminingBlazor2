using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using BminingBlazor.ViewModels.Projects;
using Models.Project;

namespace BminingBlazor.Utility
{
    public static class Extensions
    {
        public static string GetEmail(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.Name).Value;
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
