using System.Collections.Generic;
using System.Threading.Tasks;

namespace Data
{
    public interface IDataAccess
    {
        Task<List<T>> LoadData<T, U>(string sql, U parameters, string connectionString);
        Task SaveData<T>(string sql, T parameters, string connectionString);
        Task<List<T1>> LoadData<T1, T2>(string sql, object p);
        Task UpdateData<T>(string sql, T parameters, string connectionString);
        Task DeleteData<T>(string sql, T parameters, string connectionString);
    }
}