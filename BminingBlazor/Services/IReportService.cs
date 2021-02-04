using BminingBlazor.ViewModels.Report;
using BminingBlazor.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface IReportService
    {
        Task<List<ReportViewModel>> GetUserReport(int userId, DateTime from, DateTime to, int projectId);
        Task<List<ReportViewModel>> GetProjectReportTree(DateTime from, DateTime to, string codeProject);
        Task<List<ReportViewModel>> GetProjectReport(DateTime from, DateTime to, int projectId);
        Task<List<ReportViewModel>> GetProjectReportSons(DateTime from, DateTime to, int projectId);
        Task<List<MemberViewModel>> ReadMembersFromCode(string codeProject);
        Task<List<ReportViewModel>> GetUserReportFromCode(int userId, DateTime from, DateTime to, string codeProject);
        Task<List<ReportViewModel>> GetUserProjectReportSons(int userId, DateTime from, DateTime to, int projectId);
    }
}
