using BminingBlazor.ViewModels.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface IReportService
    {
        Task<List<ReportViewModel>> GetUserReport(int userId, DateTime from, DateTime to, int projectId);
        Task<List<ReportViewModel>> GetProjectReport(DateTime from, DateTime to, string codeProject);
    }
}
