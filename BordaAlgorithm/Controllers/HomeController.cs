using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using BordaAlgorithm.Models;
using BordaAlgorithm.Utilities;
using BordaAlgorithm.ViewModels;

namespace BordaAlgorithm.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public ActionResult PriorityChart()
        {
             DBBordaAlgorithmEntities db = new DBBordaAlgorithmEntities();
            List<object> dataList = new List<object>();

            //string query = @"
            //Select
            // (Select Count(Main_Priority) from Data_Survey where Main_Priority = 'LCD Size') as LCD_Size,
            // (Select Count(Main_Priority) from Data_Survey where Main_Priority = 'RAM Capacity') as RAM_Capacity,
            // (Select Count(Main_Priority) from Data_Survey where Main_Priority = 'Memory Capacity') as Memory_Capacity,
            // (Select Count(Main_Priority) from Data_Survey where Main_Priority = 'Processor Speed') as Processor_Speed,
            // (Select Count(Main_Priority) from Data_Survey where Main_Priority = 'Price') as Price
            //";

            //Dictionary<string, object> sqlParams = new Dictionary<string, object>();
            //DataTable dtTempData = new DataTable();
            //string res = BaseProgram.GetDataTable("", query, sqlParams, out dtTempData);


            DataTable dtTempData = new DataTable();
            dtTempData.Columns.Add("LCD_Size", typeof(decimal));
            dtTempData.Columns.Add("RAM_Capacity", typeof(decimal));
            dtTempData.Columns.Add("Memory_Capacity", typeof(decimal));
            dtTempData.Columns.Add("Processor_Speed", typeof(decimal));
            dtTempData.Columns.Add("Price", typeof(decimal));

            DataRow newRow = dtTempData.NewRow();
            newRow["LCD_Size"] = 0.0;
            newRow["RAM_Capacity"] = 0.0;
            newRow["Memory_Capacity"] = 0.0;
            newRow["Processor_Speed"] = 0.0;
            newRow["Price"] = 0.0;
            dtTempData.Rows.Add(newRow);

            DataTable dtPointBorda = BaseProgram.BordaAlgoritmaPoint();
            if (dtPointBorda != null && dtPointBorda.Rows.Count > 0)
            {
                double totalRows = dtPointBorda.AsEnumerable().Sum(r => r.Field<double>("Total_Rows"));
                foreach (DataRow row in dtPointBorda.Rows)
                {
                    row["Borda_Point"] = Math.Round(BaseProgram.ConvertToDouble(row["Total_Rows"].ToString()) / totalRows, 4) * 100;
                    dtTempData.Rows[0][row["Category"].ToString().Replace(' ', '_')] = BaseProgram.ConvertToDouble(row["Borda_Point"].ToString());
                }
            }


            List<GroupGeneratorHelper> data = new List<GroupGeneratorHelper>();
            data = ConvertDataTable<GroupGeneratorHelper>(dtTempData);

            List<string> labelList = new List<string>();// data.Select(r => r.tahun.ToString()).Distinct().ToList();
            labelList.Add("");
            List<string> labelData = new List<string>()
            {
                "LCD SIZE", "RAM CAPACITY", "MEMORY CAPACITY", "PROCESSOR SPEED", "PRICE"
            };
            List<string> backgroundList = new List<string>()
            {
                "rgba(68,114,196,0.7)",
                "rgba(237,125,49,0.7)",
                "rgba(238,45,49,0.7)",
                "rgba(45,125,49,0.7)",
                "rgba(222,125,200,0.7)"
            };

            List<object> dData = new List<object>();

            for (int i = 0; i < labelData.Count; i++)
            {
                decimal[] vData = new decimal[5];

                if (i == 0)
                {
                    vData = data.Select(a => a.LCD_Size).ToArray();
                }
                else if (i == 1)
                {
                    vData = data.Select(a => a.RAM_Capacity).ToArray();
                }
                else if (i == 2)
                {
                    vData = data.Select(a => a.Memory_Capacity).ToArray();
                }
                else if (i == 3)
                {
                    vData = data.Select(a => a.Processor_Speed).ToArray();
                }
                else if (i == 4)
                {
                    vData = data.Select(a => a.Price).ToArray();
                }

                var d = new
                {
                    label = labelData[i],
                    backgroundColor = backgroundList[i],
                    data = vData
                };
                dataList.Add(d);
            }

            return Json(new { labels = labelList, data = dataList },JsonRequestBehavior.AllowGet);
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
    public class GroupGeneratorHelper
    {
        public decimal LCD_Size { get; set; }
        public decimal RAM_Capacity { get; set; }
        public decimal Memory_Capacity { get; set; }
        public decimal Processor_Speed { get; set; }
        public decimal Price { get; set; }
        //public int tahun { get; set; }
    }
}