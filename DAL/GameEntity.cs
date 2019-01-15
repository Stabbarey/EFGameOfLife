namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Contains all the Games
    /// </summary>
    [Table("SavedGames")]
    public partial class GameEntity
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [StringLength(25)]
        public string Name { get; set; }
        public int BoardGridGameID { get; set; }
        public int Generations { get; set; }
        public bool? Infinite { get; set; }
    }
}
