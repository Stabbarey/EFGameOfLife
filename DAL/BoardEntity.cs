namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// Contains all the "frames" of the Game
    /// </summary>
    [Table("BoardGrid")]
    public partial class BoardEntity
    {
        public int Id { get; set; }

        public string Grid { get; set; }

        public int Generation { get; set; }

        public int GameId { get; set; }
    }
}
