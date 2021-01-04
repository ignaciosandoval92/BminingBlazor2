using BminingBlazor.ViewModels.SecurityReflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface ISecurityReflectionDataService
    {
        Task<List<SecurityReflectionViewModel>> ReadSecurityReflection();
        Task EditTopic(int securityId, string topic);
    }
}
