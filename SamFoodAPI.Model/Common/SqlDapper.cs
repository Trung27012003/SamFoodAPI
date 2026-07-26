using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamFoodAPI.Model.Common
{
    public static class SqlDapper<T> where T : class, new()
    {
        static string connectionString = Config.ConnectionString;
        static int commandTimeout = 200;
        public static async Task<object> ProcedureToListAsync(string procedureName, object param)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: commandTimeout);
                    var result = (await data.ReadAsync()).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public static async Task<List<T>> ProcedureToListModelAsync(string procedureName, object param)
        {
           
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: commandTimeout);
                    var result = (await data.ReadAsync<T>()).ToList();
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<(List<T1>, List<T2>)> QueryMultipleAsync<T1, T2>(
         string procedureName,
         object? parameters = null,
         IDbTransaction? transaction = null)
        {
            var connection = new SqlConnection(connectionString);
            using var multi = await connection.QueryMultipleAsync(
                procedureName,
                parameters,
                transaction,
                commandType: CommandType.StoredProcedure);

            return (
                multi.Read<T1>().AsList(),
                multi.Read<T2>().AsList()
            );
        }

        public static async Task<T> ProcedureToModelAsync(string procedureName, object param)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var data = await connection.QueryMultipleAsync(procedureName, param, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: commandTimeout);
                    var result = await data.ReadSingleOrDefaultAsync<T>();
                    return result ?? new T();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static async Task<int> ExecuteStoredProcedure(
        string procedureName,
        object parameters = null,
        int? commandTimeout = null)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return await connection.ExecuteAsync(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    commandTimeout: commandTimeout
                );
            }
        }
    }
}
