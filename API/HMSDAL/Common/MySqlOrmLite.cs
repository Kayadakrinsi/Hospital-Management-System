using Microsoft.Extensions.Configuration;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using System.Data;

namespace HMSDAL.Common
{
    /// <summary>
    /// MySql OrmLite Connection Provider
    /// </summary>
    public class MySqlOrmLite
    {
        /// <summary>
        /// Instance of IDbConnectionFactory
        /// </summary>
        private readonly IDbConnectionFactory _dbFactory;

        /// <summary>
        /// Sets db factory instance
        /// </summary>
        public MySqlOrmLite()
        {
            // Load configuration
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("Connstr");

            // Set dialect
            OrmLiteConfig.DialectProvider = MySqlDialect.Provider;

            // Initialize factory
            _dbFactory = new OrmLiteConnectionFactory(connectionString, MySqlDialect.Provider);
        }

        /// <summary>
        /// Opens and returns a database connection
        /// </summary>
        /// <returns></returns>
        public IDbConnection Open() => _dbFactory.Open();
    }
}
