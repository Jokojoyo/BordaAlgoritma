//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BordaAlgorithm.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public long Uniq { get; set; }
        public string Username { get; set; }
        public string Longname { get; set; }
        public Nullable<bool> Is_Admin { get; set; }
        public string Career { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> Last_Login { get; set; }
        public string Create_By { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Edit_By { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Delete_By { get; set; }
        public Nullable<System.DateTime> Delete_Date { get; set; }
        public Nullable<bool> Is_Deleted { get; set; }
    }
}
