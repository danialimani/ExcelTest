using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelExam.Data.DatabaseModel
{
    public class City
    {
        [Key]
        public string CityName { get; set; }
        //public ICollection<Sale> Sales { get; set; }
        //public City()
        //{
        //    Sales = new Collection<Sale>();
        //}
    }
}
