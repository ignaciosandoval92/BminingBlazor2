using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;

namespace Data
{
    public class DataAccess : IDataAccess
    {
        public async Task<List<T>> LoadData<T, U>(string sql, U parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);
                return rows.ToList();
            }
        }
        public Task<List<T1>> LoadData<T1, T2>(string sql, object p)
        {
            throw new NotImplementedException();
        }



        public  Task SaveData<T>(string sql, T parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
               return connection.ExecuteAsync(sql, parameters);
                
            }
        }

        public Task UpdateData<T>(string sql, T parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                return connection.ExecuteAsync(sql, parameters);

            }
        }

        public Task DeleteData<T>(string sql, T parameters, string connectionString)
        {
            using (IDbConnection connection = new MySqlConnection(connectionString))
            {
                return connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
