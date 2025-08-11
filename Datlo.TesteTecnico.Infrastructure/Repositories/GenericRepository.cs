using Npgsql;

namespace Datlo.TesteTecnico.Infrastructure.Repositories
{
    public class GenericRepository<T>
    {
        protected readonly NpgsqlConnection Connection;

        public GenericRepository(NpgsqlConnection connection)
        {
            ArgumentNullException.ThrowIfNull(connection);

            Connection = connection;
        }
    }
}
