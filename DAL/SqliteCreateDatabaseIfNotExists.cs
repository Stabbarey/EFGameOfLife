using System.Data.Entity;

namespace DAL
{
    internal class SqliteCreateDatabaseIfNotExists<T>
    {
        private DbModelBuilder modelBuilder;

        public SqliteCreateDatabaseIfNotExists(DbModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }

    }
}