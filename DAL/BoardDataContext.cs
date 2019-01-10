namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using SQLite.CodeFirst;

    public partial class BoardDataContext : DbContext
    {
        public BoardDataContext()
            : base("name=BoardDataContext")
        {
        }

        public virtual DbSet<BoardGrid> BoardGrid { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BoardGrid>().ToTable("BoardGrid");
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<BoardDataContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
        }
    }
}
