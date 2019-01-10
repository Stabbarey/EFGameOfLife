using System.Data.Entity;

namespace DAL
{
    internal class SqliteCreateDatabaseIfNotExists2<T>
    {
        private DbModelBuilder modelBuilder;

        public SqliteCreateDatabaseIfNotExists2(DbModelBuilder modelBuilder)
        {
            this.modelBuilder = modelBuilder;
        }

    }
}