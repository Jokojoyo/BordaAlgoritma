using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BordaAlgorithm.Models;
using BordaAlgorithm.Utilities;

namespace BordaAlgorithm.ViewModels
{
    public class DataSurveyViewModel
    {
        public DataSurveyViewModel() { }
        public DataSurveyViewModel(DBBordaAlgorithmEntities db, long id)
        {
            Data_Survey model = db.Data_Survey.Find(id);
            if (model == null) return;
            BaseProgram.CopyProperties(typeof(Data_Survey), model, typeof(DataSurveyViewModel), this);
        }

        public DataSurveyViewModel(Data_Survey model)
        {
            BaseProgram.CopyProperties(typeof(Data_Survey), model, typeof(DataSurveyViewModel), this);
            mode = Constants.FORM_MODE_UNCHANGED;
        }
        public string mode { get; set; }
        public string result { get; set; }
        public long Uniq { get; set; }

        [Required]
        [Display(Name = "Main Priority")]
        public string Main_Priority { get; set; }

        [Required]
        [Display(Name = "Second Priority")]
        public string Second_Priority { get; set; }

        [Required]
        [Display(Name = "Third Priority")]
        public string Third_Priority { get; set; }

        [Required]
        [Display(Name = "Fourth Priority")]
        public string Fourth_Priority { get; set; }

        [Required]
        [Display(Name = "Fifth Priority")]
        public string Fifth_Priority { get; set; }

    }
}