using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using BordaAlgorithm.Models;
using BordaAlgorithm.Utilities;

namespace BordaAlgorithm.ViewModels
{
    public class DataUserViewModel
    {
        public DataUserViewModel() { }
        public DataUserViewModel(DBBordaAlgorithmEntities db, long id)
        {
            User model = db.Users.Find(id);
            if (model == null) return;
            BaseProgram.CopyProperties(typeof(User), model, typeof(DataUserViewModel), this);
        }

        public DataUserViewModel(User model)
        {
            BaseProgram.CopyProperties(typeof(User), model, typeof(DataUserViewModel), this);
            mode = Constants.FORM_MODE_UNCHANGED;
        }

        public string mode { get; set; }
        public string result { get; set; }
        public long Uniq { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Username { get; set; }


        [Required]
        [Display(Name = "Name")]
        public string Longname { get; set; }

        public Nullable<bool> Is_Admin { get; set; }

        [Required]
        [Display(Name = "Career")]
        public string Career { get; set; }

        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm")]
        public string ConfirmPassword { get; set; }

        public DateTime Created_Date { get; set; }
        public string Created_By { get; set; }
        public DateTime? Edited_Date { get; set; }
        public string Edited_By { get; set; }
        public bool Is_Deleted { get; set; }
        public DateTime? Deleted_Date { get; set; }
        public string Deleted_By { get; set; }
    }
}