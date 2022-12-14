using BordaAlgorithm.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BordaAlgorithm.Models;
using BordaAlgorithm.ViewModels;
using System.IO;

namespace BordaAlgorithm.Controllers
{
    public class DataLaptopController : Controller
    {
        DBBordaAlgorithmEntities db = new DBBordaAlgorithmEntities();
        // GET: DataLaptop
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LoadData(string draw = null, string start = null, string length = null, OrderClass[] order = null, ColumnClass[] columns = null, string[] filter = null, bool showDeleted = false)
        {
            try
            {
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int filteredTotal = 0, recordsTotal = 0;
                string orderBy = string.Join(",", order.Select(r => (columns[r.column].data + " " + r.dir)));
                if (orderBy.ToUpper().Contains("DATE") || orderBy.ToUpper().Contains("LOGIN"))
                {
                    string columnOrder = orderBy.Split(' ')[0].ToString();
                    string metodeOrder = orderBy.Split(' ')[1].ToString();

                    columnOrder = "Convert (date, " + columnOrder + ", 103)";
                    orderBy = columnOrder + " " + metodeOrder;
                }
                string whereQuery = showDeleted ? "" : " AND ISNULL(TBL.Is_Deleted,0)=0 ";
                string deleteQuery = showDeleted ? "" : " AND ISNULL(TBL.Is_Deleted,0)=0 ";
                string mainQuery = @"
                                    Select * FROM (
                                        select TBL.Uniq, TBL.Laptop_Brand,TBL.Laptop_Type,TBL.Currency,TBL.Price,
                                            convert(varchar(10), TBL.Create_Date, 103) as Create_Date,
                                            convert(varchar(10), TBL.Edit_Date, 103) as Edit_Date,
                                            convert(varchar(10), TBL.Delete_Date, 103) as Delete_Date,
	                                        TBL.Create_By,TBL.Edit_By,TBL.Delete_By, tbl.Is_Deleted
                                        from Laptops TBL
                                    ) TBL
                                    WHERE 1 = 1 {0}  
                                    ORDER BY {3} {2} {1} ";
                string countQuery = @"
                                    Select count(*) as Count_ FROM (
                                        select TBL.Uniq, TBL.Laptop_Brand,TBL.Laptop_Type,TBL.Currency,TBL.Price,
                                            convert(varchar(10), TBL.Create_Date, 103) as Create_Date,
                                            convert(varchar(10), TBL.Edit_Date, 103) as Edit_Date,
                                            convert(varchar(10), TBL.Delete_Date, 103) as Delete_Date,
	                                        TBL.Create_By,TBL.Edit_By,TBL.Delete_By, tbl.Is_Deleted
                                        from Laptops TBL
                                    ) TBL
                                    WHERE 1 = 1 {0} ";
                List<SqlParameter> parameters = new List<SqlParameter>();
                Dictionary<string, object> sqlParams = new Dictionary<string, object>();

                if (filter != null && filter.Length > 0)
                {
                    for (int i = 0; i < filter.Length; i++)
                    {
                        if (string.IsNullOrEmpty(filter[i]))
                            continue;
                        string columnName = columns[i].data;
                        string tablePrefix = "TBL.";
                        string keyword = "";
                        keyword = "%" + filter[i] + "%";
                        whereQuery += " AND " + tablePrefix + columnName + " LIKE @" + columnName;
                        sqlParams.Add(columnName, keyword);
                    }
                }
                string query = string.Format(mainQuery, whereQuery, (pageSize > 0 ? "fetch next " + pageSize.ToString() + " rows only" : ""), (skip > -1 ? "offset " + skip.ToString() + " rows" : ""), (string.IsNullOrEmpty(orderBy) ? "req_date" : orderBy));
                string queryCount = string.Format(countQuery, whereQuery);
                string totalCount = string.Format(countQuery, " 1=1 ");

                DataTable q = new DataTable(), qc = new DataTable(), qt = new DataTable();
                string sError = BaseProgram.GetDataTable("", query, sqlParams, out q);

                var listOfData_ =
                    q.Rows.Cast<DataRow>().Select(r => new
                    {
                        Uniq = r["Uniq"].ToString(),
                        Laptop_Brand = r["Laptop_Brand"].ToString(),
                        Laptop_Type = r["Laptop_Type"].ToString(),
                        Currency = r["Currency"].ToString(),
                        Price = r["Price"].ToString(),
                        Create_By = r["Create_By"].ToString(),
                        Create_Date = r["Create_Date"].ToString(),
                        Edit_By = r["Edit_By"].ToString(),
                        Edit_Date = r["Edit_Date"].ToString(),
                        Delete_By = r["Delete_By"].ToString(),
                        Delete_Date = r["Delete_Date"].ToString(),
                        Is_Deleted = r["Is_Deleted"]
                    });
                sError = BaseProgram.GetDataTable("", queryCount, sqlParams, out qc);
                filteredTotal = qc.Rows.Count > 0 ? int.Parse(qc.Rows[0][0].ToString()) : 0;

                query = @"SELECT COUNT(*) as a FROM [Users] where 1 = 1 " + deleteQuery + "";
                sError = BaseProgram.GetDataTable("", query, sqlParams, out qt);

                recordsTotal = qt.Rows.Count > 0 ? int.Parse(qt.Rows[0][0].ToString()) : 0;
                return Json(new { draw = draw, recordsFiltered = filteredTotal, recordsTotal = recordsTotal, data = listOfData_ });

            }
            catch (Exception exc)
            {
                var a = exc.Message;
                throw;
            }
        }

        public ActionResult Editor(string mode, long? uniq)
        {
            DataLaptopViewModel viewModel = new DataLaptopViewModel();
            if (mode != Constants.FORM_MODE_CREATE && uniq != null)
            {
                viewModel = new DataLaptopViewModel(db, uniq.Value);
                ViewBag.tempURL = VirtualPathUtility.ToAbsolute("~/UploadedFile/" + viewModel.Laptop_Image);
            }
            else if (mode != Constants.FORM_MODE_CREATE && uniq == null)
                return RedirectToAction("Index");
            viewModel.mode = mode;
            return View("Editor", viewModel);
        }

        [HttpPost]
        public ActionResult Editor(DataLaptopViewModel model, HttpPostedFileBase file = null)
        {
            ModelState.Remove("result");
            string mode = model.mode;
            var a = ModelState.AsEnumerable().Where(r => r.Value.Errors.Count > 0).ToList();
            if (ModelState.IsValid)
            {
                Laptop newModel = null;
                try
                {
                    if (string.IsNullOrEmpty(mode))
                    {
                        throw new Exception("Invalid mode");
                    }

                    if (mode == Constants.FORM_MODE_CREATE)
                        newModel = new Laptop();
                    else
                        newModel = db.Laptops.Find(model.Uniq);

                    if (mode == Constants.FORM_MODE_CREATE)
                    {
                        newModel.Create_By = User.Identity.GetUserDataByKey("Username");
                        newModel.Create_Date = DateTime.Now;
                        db.Entry(newModel).State = System.Data.Entity.EntityState.Added;
                    }
                    else if (mode == Constants.FORM_MODE_EDIT)
                    {
                        newModel.Edit_By = User.Identity.GetUserDataByKey("Username");
                        newModel.Edit_Date = DateTime.Now;
                        db.Entry(newModel).State = System.Data.Entity.EntityState.Modified;
                    }
                    else if (mode == Constants.FORM_MODE_DELETE)
                    {
                        newModel.Delete_By = User.Identity.GetUserDataByKey("Username");
                        newModel.Delete_Date = DateTime.Now;
                        newModel.Is_Deleted = true;

                        db.Entry(newModel).State = System.Data.Entity.EntityState.Modified;
                    }

                    if (mode != Constants.FORM_MODE_DELETE)
                    {
                        string fileName = "";
                        if(file!=null && file.ContentLength>0)
                        {
                            // extract only the filename
                            fileName = model.Laptop_Brand+"__"+model.Laptop_Type+"__"+Path.GetFileName(file.FileName);
                            // store the file inside ~/App_Data/uploads folder
                            var path = Path.Combine(Server.MapPath("~/UploadedFile"), fileName);
                            file.SaveAs(path);
                            newModel.Laptop_Image = fileName;
                        }
                        newModel.Laptop_Brand = model.Laptop_Brand;
                        newModel.Laptop_Type = model.Laptop_Type;
                        newModel.LCD_Height = model.LCD_Height;
                        newModel.LCD_Width = model.LCD_Width;
                        newModel.Ram_Capacity = model.Ram_Capacity;
                        newModel.Memory_Capacity = model.Memory_Capacity;
                        newModel.Processor_Speed = model.Processor_Speed;
                        newModel.Processor_UOM = "GHz";
                        newModel.Currency = "IDR";
                        newModel.Price = model.Price;
                        newModel.Remarks = model.Remarks;
                        //model.Laptop_Image_Helper = VirtualPathUtility.ToAbsolute("~/UploadedFile/" + newModel.Laptop_Image);
                        ViewBag.tempURL = VirtualPathUtility.ToAbsolute("~/UploadedFile/" + newModel.Laptop_Image);
                    }

                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            if (mode == Constants.FORM_MODE_CREATE)
                            {
                                var existed = db.Laptops.Where(r => r.Laptop_Brand.ToString().ToUpper() == model.Laptop_Brand.ToString().ToUpper() 
                                && r.Laptop_Type.ToString().ToUpper() == model.Laptop_Type.ToString().ToUpper()).FirstOrDefault();
                                if (existed != null)
                                    throw new Exception("Laptop with Brand   :  " + existed.Laptop_Brand + " and Type : "+ existed .Laptop_Type+ " is Existed!");
                            }
                            db.SaveChanges();
                            trans.Commit();
                            model.result = "OK";
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw new Exception(ex.Message);
                        }
                    }
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError(Constants._ERROR, exc.Message.Replace(Environment.NewLine, " ").Replace("'", ""));
                }
            }
            else
            {
                ModelState.AddModelError(Constants._ERROR, "Error(s) occurred. Please refer to field errors");
            }
            return View(model);
        }
    }
}