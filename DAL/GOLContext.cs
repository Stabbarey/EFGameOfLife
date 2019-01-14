namespace DAL
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class GOLContext : DbContext
    {
        public GOLContext()
            : base("name=BoardDataContext")
        {
        }

        public virtual DbSet<BoardEntity> BoardGrid { get; set; }
        public virtual DbSet<GameEntity> SavedGames { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
