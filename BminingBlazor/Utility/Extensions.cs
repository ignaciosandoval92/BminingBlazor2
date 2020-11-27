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
                ProjectName = projectModel.Nombre_Proyecto,
                ProjectCode = projectModel.Cod_Proyecto,
                ClientId = projectModel.Id_Cliente,
                Payments = new List<PaymentViewModel>(),
                Id = projectModel.Id_proyecto,
                StartDate = projectModel.Fecha_Inicio, // TODO: Revisar Los UTC
                EndDate = projectModel.Fecha_Fin
            };
        }

    }
}
