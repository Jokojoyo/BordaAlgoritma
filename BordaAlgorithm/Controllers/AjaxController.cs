using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BordaAlgorithm.Models;
using BordaAlgorithm.Utilities;
using BordaAlgorithm.ViewModels;

namespace BordaAlgorithm.Controllers
{
    public class AjaxController : Controller
    {
        DBBordaAlgorithmEntities db = new DBBordaAlgorithmEntities();
        // GET: Ajax
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult CareerList(string q = "", int page = 0, bool init= false)
        {
            int fromIdx = page > 0 ? (page * 10) : 0;
            int toIdx = page > 0 ? page * 10 : 10;
            List<Career_List> listData = new List<Career_List>();
            listData = db.Career_List.Where(r => (string.IsNullOrEmpty(q) || (r.Career_Name).ToLower().Contains(q.ToLower())) && r.Is_Deleted != true).OrderBy(a => a.Career_Name).ToList();

            var data = listData.Skip(fromIdx)
                .Take(toIdx)
                .Select(r => new { id = r.Career_Name, text = r.Career_Name, name = r.Career_Name })
                .ToList();
            return Json(new { items = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LaptopBrandList(string q = "", int page = 0, bool init = false)
        {
            int fromIdx = page > 0 ? (page * 10) : 0;
            int toIdx = page > 0 ? page * 10 : 10;
            List<Laptops_Brand_List> listData = new List<Laptops_Brand_List>();
            listData = db.Laptops_Brand_List.Where(r => (string.IsNullOrEmpty(q) || (r.Brand).ToLower().Contains(q.ToLower()))).OrderBy(a => a.Brand).ToList();

            var data = listData.Skip(fromIdx)
                .Take(toIdx)
                .Select(r => new { id = r.Brand, text = r.Brand, name = r.Brand })
                .ToList();
            return Json(new { items = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult BordaSubjectList(string q = "", int page = 0, bool init = false)
        {
            int fromIdx = page > 0 ? (page * 10) : 0;
            int toIdx = page > 0 ? page * 10 : 10;
            List<Borda_Subject_List> listData = new List<Borda_Subject_List>();
            listData = db.Borda_Subject_List.Where(r => (string.IsNullOrEmpty(q) || (r.Subject_Desc).ToLower().Contains(q.ToLower()))).OrderBy(a => a.Subject_Desc).ToList();

            var data = listData.Skip(fromIdx)
                .Take(toIdx)
                .Select(r => new { id = r.Subject_Desc, text = r.Subject_Desc, name = r.Subject_Desc })
                .ToList();
            return Json(new { items = data }, JsonRequestBehavior.AllowGet);
        }
    }
}