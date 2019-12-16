using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication3.Models.ViewModels
{
    public class MonthlyReportViewModel
    {
        public int Account { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime Date { get; set; }
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double Credit { get; set; }
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double Debit { get; set; }
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public double Balance { get; set; }
    }
}
