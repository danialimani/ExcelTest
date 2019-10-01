using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelExam.Data.DatabaseModel
{
    public class Sale
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string CityName { get; set; }
        [ForeignKey("CityName")]
        public City City { get; set; }
        public string PersonFullName { get; set; }
        public string ProductCode { get; set; }
        public string NameProduct { get; set; }
        public float Price { get; set; }
    }
}
