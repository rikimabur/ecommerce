using Application.Domain.Entities;
using Application.Infrastructure.Persistence;
using Dapper;

namespace Application.Infrastructure.Repositories
{
    public interface IUserRepository : IGenericRepository<User,int>
    {
        Task<User> GetByUserName(string userName);
    }

    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<User> GetByUserName(string userName)
        {

            User result;
            try
            {
                string tableName = GetTableName();
                string keyColumn = GetKeyColumnName();
                string query = $"SELECT {GetColumnsAsProperties()} FROM {tableName} WHERE {keyColumn} = '{userName}'";

                result = await _connection.QueryFirstOrDefaultAsync<T>(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching a record from db: ${ex.Message}");
                throw new Exception("Unable to fetch data. Please contact the administrator.");
            }
            finally
            {
                _connection.Close();
            }

            return result;
        }
    }
}
