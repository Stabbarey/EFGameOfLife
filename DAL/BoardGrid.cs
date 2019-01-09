namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BoardGrid")]
    public partial class BoardGrid
    {
        public int Id { get; set; }

        public byte[] Grid { get; set; }
    }
}
