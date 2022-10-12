using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BordaAlgorithm.Models;
using BordaAlgorithm.ViewModels;

namespace BordaAlgorithm.Controllers
{
    public class DataSurveyController : Controller
    {
        DBBordaAlgorithmEntities db = new DBBordaAlgorithmEntities();
        // GET: DataSurvey
        public ActionResult Index()
        {
            string username = User.Identity.GetUserDataByKey("Username");
            Data_Survey dtSurvey = db.Data_Survey.Where(r => r.Username == username).FirstOrDefault();
            DataSurveyViewModel survey = new DataSurveyViewModel();
            if (dtSurvey!= null)
                survey = new DataSurveyViewModel(db, dtSurvey.Uniq);
            return View(survey);
        }

        [HttpPost]
        public ActionResult Index(DataSurveyViewModel model)
        {
            ModelState.Remove("result");
            var a = ModelState.AsEnumerable().Where(r => r.Value.Errors.Count > 0).ToList();
            if (ModelState.IsValid)
            {
                Data_Survey newModel = null;
                try
                {
                    if (model.Uniq == 0)
                    {
                        newModel = new Data_Survey();
                        db.Entry(newModel).State = System.Data.Entity.EntityState.Added;
                    }
                    else
                    {
                        newModel = db.Data_Survey.Find(model.Uniq);
                        db.Entry(newModel).State = System.Data.Entity.EntityState.Modified;
                    }

                    //check if survey is valid
                    List<string> listSurvey = new List<string>();
                    listSurvey.Add(model.Main_Priority);
                    listSurvey.Add(model.Second_Priority);
                    listSurvey.Add(model.Third_Priority);
                    listSurvey.Add(model.Fourth_Priority);
                    listSurvey.Add(model.Fifth_Priority);

                    for(int i =0;i<listSurvey.Count-1;i++)
                    {
                        for (int j = i+1;j<listSurvey.Count;j++)
                        {
                            if(listSurvey[i]==listSurvey[j])
                            {
                                throw new Exception("You select duplicate category "+ listSurvey[i] + ", Please re-check Your Survey!");
                            }
                        }
                    }
                    //end check if survey is valid

                    newModel.Username = User.Identity.GetUserDataByKey("Username");
                    newModel.Main_Priority = model.Main_Priority;
                    newModel.Second_Priority = model.Second_Priority;
                    newModel.Third_Priority = model.Third_Priority;
                    newModel.Fourth_Priority = model.Fourth_Priority;
                    newModel.Fifth_Priority = model.Fifth_Priority;

                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            db.SaveChanges();
                            trans.Commit();
                            model.result = "OK";
                            return RedirectToAction("Index", "Catalog", new { method = "UserRecommendation"});
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