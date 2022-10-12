using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BordaAlgorithm.Models
{
    public class DataLaptopsModel
    {
        public List<Laptop> LaptopsView { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
    }
}