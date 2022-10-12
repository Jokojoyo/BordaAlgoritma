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

namespace BordaAlgorithm.Controllers
{
    public class CatalogController : Controller
    {
        // GET: Catalog
        public ActionResult Index(string method)
        {
            ViewBag.method = method;
            return View();
        }

        [HttpPost]
        public JsonResult AjaxMethod(int pageIndex, string method)
        {

            DBBordaAlgorithmEntities db = new DBBordaAlgorithmEntities();
            DataLaptopsModel model = new DataLaptopsModel();
            model.PageIndex = pageIndex;
            model.PageSize = 8;
            model.RecordCount = db.Laptops.Count();
            int startIndex = (pageIndex - 1) * model.PageSize;

            DataTable dtLaptop = new DataTable();
            string query = "Select * FROM Laptops ";
            if(method == "UserRecommendation")
            {
                string username = User.Identity.GetUserDataByKey("Username");
                DataTable dtSurvey = new DataTable();
                string querySurvey = "Select top 1 * FROM Data_Survey where username = '"+username+"' order by uniq desc";
                string sErrorSurvey = BaseProgram.GetDataTable("", querySurvey, new Dictionary<string, object>(), out dtSurvey);

                if(dtSurvey.Rows.Count>0)
                {
                    query += " Order by ";

                    foreach(DataColumn col in dtSurvey.Columns)
                    {
                        if(col.ColumnName=="Uniq" || col.ColumnName == "Username")
                        {
                            continue;
                        }
                        else if(dtSurvey.Rows[0][col.ColumnName].ToString() == "Price")
                        {
                            query += " Price asc,";
                        }
                        else if(dtSurvey.Rows[0][col.ColumnName].ToString() == "LCD Size")
                        {
                            query += " LCD_Height * LCD_Width desc,";
                        }
                        else
                        {
                            query += " "+ dtSurvey.Rows[0][col.ColumnName].ToString().Replace(' ','_') + " desc,";
                        }
                    }
                    query = query.Remove(query.Length - 1, 1);
                }
            }
            else if(method == "bordaAlgoritma")
            {

                DataTable dtPointBorda = BaseProgram.BordaAlgoritmaPoint();
                //calculate borda point
                if(dtPointBorda!= null && dtPointBorda.Rows.Count>0)
                {
                    double totalRows = dtPointBorda.AsEnumerable().Sum(r => r.Field<double>("Total_Rows"));
                    DataTable dtOrderd = dtPointBorda.AsEnumerable().OrderByDescending(r => r.Field<double>("Total_Rows")).CopyToDataTable();
                    query += " Order by ";
                    foreach (DataRow row in dtOrderd.Rows)
                    {
                        row["Borda_Point"] = Math.Round(BaseProgram.ConvertToDouble(row["Total_Rows"].ToString()) / totalRows,4) * 100;

                        if(row["Category"].ToString()== "Price")
                        {
                            query += " Price asc,";
                        }
                        else if (row["Category"].ToString() == "LCD Size")
                        {
                            query += "  LCD_Height * LCD_Width desc,";
                        }
                        else
                        {
                            query += " "+row["Category"].ToString().Replace(' ', '_') + " desc,"; 
                        }
                    }

                    query = query.Remove(query.Length - 1, 1);
                }
            }
            string sError = BaseProgram.GetDataTable("", query, new Dictionary<string, object>(), out dtLaptop);
            DataRow[] dtView = dtLaptop.AsEnumerable().Where(r => r.Field<bool?>("Is_Deleted") != true).Skip(startIndex).Take(model.PageSize).ToArray();
            List<Laptop> LaptopList = new List<Laptop>();
            for(int i = 0;i<dtView.Length;i++)
            {
                Laptop newLaptop = new Laptop();
                newLaptop.Uniq = Int64.Parse(dtView[i]["Uniq"].ToString());
                newLaptop.Laptop_Brand = dtView[i]["Laptop_Brand"].ToString();
                newLaptop.Laptop_Type = dtView[i]["Laptop_Type"].ToString();
                newLaptop.LCD_Height = BaseProgram.ConvertToDouble(dtView[i]["LCD_Height"].ToString());
                newLaptop.LCD_Width = BaseProgram.ConvertToDouble(dtView[i]["LCD_Width"].ToString());
                newLaptop.Ram_Capacity = BaseProgram.ConvertToDouble(dtView[i]["Ram_Capacity"].ToString());
                newLaptop.Memory_Capacity = BaseProgram.ConvertToDouble(dtView[i]["Memory_Capacity"].ToString());
                newLaptop.Processor_Speed = BaseProgram.ConvertToDouble(dtView[i]["Processor_Speed"].ToString());
                newLaptop.Processor_UOM = dtView[i]["Processor_UOM"].ToString();
                newLaptop.Currency = dtView[i]["Currency"].ToString();
                newLaptop.Price = BaseProgram.ConvertToDouble(dtView[i]["Price"].ToString());
                newLaptop.Laptop_Image = dtView[i]["Laptop_Image"].ToString();
                newLaptop.Remarks = dtView[i]["Remarks"].ToString();
                LaptopList.Add(newLaptop);
            }

            model.LaptopsView = LaptopList;
            //model.LaptopsView = (db.Laptops.Where(r=>r.Is_Deleted!=true))
            //                .OrderBy(r=>r.Laptop_Brand).ThenByDescending(r=>r.Price)
            //                .Skip(startIndex)
            //                .Take(model.PageSize).ToList();
            return Json(model);
        }
    }
}