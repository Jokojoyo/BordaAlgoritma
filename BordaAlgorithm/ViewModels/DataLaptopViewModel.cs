using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BordaAlgorithm.Models;
using BordaAlgorithm.Utilities;

namespace BordaAlgorithm.ViewModels
{
    public class DataLaptopViewModel
    {
        public DataLaptopViewModel() { }
        public DataLaptopViewModel (DBBordaAlgorithmEntities db, long id)
        {
            Laptop model = db.Laptops.Find(id);
            if (model == null) return;
            BaseProgram.CopyProperties(typeof(Laptop), model, typeof(DataLaptopViewModel), this);
        }

        public DataLaptopViewModel(Laptop model)
        {
            BaseProgram.CopyProperties(typeof(Laptop), model, typeof(DataLaptopViewModel), this);
            mode = Constants.FORM_MODE_UNCHANGED;
        }
        public string mode { get; set; }
        public string result { get; set; }
        public long Uniq { get; set; }


        [Required]
        [Display(Name = "Brand")]
        public string Laptop_Brand { get; set; }

        [Required]
        [Display(Name = "Type")]
        public string Laptop_Type { get; set; }


        [Required]
        [Display(Name = "Display Size Height")]
        public double LCD_Height { get; set; }


        [Required]
        [Display(Name = "Width")]
        public double LCD_Width { get; set; }


        [Required]
        [Display(Name = "RAM")]
        public double Ram_Capacity { get; set; }
        public string Ram_Uom { get; set; }


        [Required]
        [Display(Name = "HDD")]
        public double Memory_Capacity { get; set; }


        [Required]
        [Display(Name = "Processor Max Frequency")]
        public double Processor_Speed { get; set; }

        public string Processor_UOM { get; set; }

        public string Currency { get; set; }


        [Required]
        [Display(Name = "Price")]
        public double Price { get; set; }


        [Display(Name = "Image")]
        public string Laptop_Image { get; set; }
        public string Laptop_Image_Helper { get; set; }


        [Display(Name = "Remarks")]
        public string Remarks { get; set; }


        public DateTime Create_Date { get; set; }
        public string Create_By { get; set; }
        public DateTime? Edit_Date { get; set; }
        public string Edit_By { get; set; }
        public bool Is_Deleted { get; set; }
        public DateTime? Delete_Date { get; set; }
        public string Delete_By { get; set; }
    }
}