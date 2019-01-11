namespace DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class SaveGameData
    {
        public int Id { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        [StringLength(25)]
        public string Name { get; set; }
    }
}
