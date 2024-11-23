using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoadDWVentas.Data.Entities.DwVentas
{
    [Table("Dim_Tiempo")]
    public class DimTiempo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TiempoID { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int Año { get; set; }

        [Required]
        public int Mes { get; set; }
    }
}
