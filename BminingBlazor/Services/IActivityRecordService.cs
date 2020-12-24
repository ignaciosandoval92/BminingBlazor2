using BminingBlazor.ViewModels.ActivityRecord;
using System.Threading.Tasks;

namespace BminingBlazor.Services
{
    public interface IActivityRecordService
    {
        Task<int> CreateActivityRecordAsync(string title, int creatorId, int projectId);
        Task<ActivityRecordViewModel> GetActivityRecord(int id);
        Task<DashboardActivityRecordViewModel> GetActivityRecords();
        Task DeleteActivityRecord(int id);
        Task EditActivityRecord(ActivityRecordViewModel activityRecord);
        Task AddActivityRecordMember(ActivityRecordMemberViewModel activityRecordMember);
        Task DeleteActivityRecordMember(int id);
        Task AddActivityRecordCommitment(ActivityRecordCommitmentViewModel activityRecordCommitment);
        Task DeleteActivityRecordCommitment(int id);
    }
}