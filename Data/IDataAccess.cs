using System.Collections.Generic;
using System.Threading.Tasks;
using SqlKata.Execution;

namespace Data
{
    public interface IDataAccess
    {
        Task<List<T>> LoadData<T, U>(string sql, U parameters, string connectionString);
        Task SaveData<T>(string sql, T parameters, string connectionString);
        Task UpdateData<T>(string sql, T parameters, string connectionString);
        Task DeleteData<T>(string sql, T parameters, string connectionString);
        QueryFactory GetQueryFactory(string connectionString);
    }
}