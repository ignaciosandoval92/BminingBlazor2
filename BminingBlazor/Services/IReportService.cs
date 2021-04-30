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
        Task<List<ReportViewModel>> GetProjectReport(DateTime from, DateTime to, int projectId);
        Task<List<ReportViewModel>> GetHoursChargedReport(DateTime from, DateTime to);
        Task<List<ReportViewModel>> GetHoursApprovedReport(DateTime from, DateTime to);
        Task<List<ReportViewModel>> GetNotChargedReport(DateTime from, DateTime to);
        Task<List<ReportViewModel>> GetManagerNotApprovedHoursReport(DateTime from, DateTime to);
    }
}
