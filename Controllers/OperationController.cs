using ConfigCenterApp.Models.Entity;
using IntegrateWebApp.Models.Database;
using IntegrateWebApp.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace IntegrateWebApp.Controllers
{
    public class OperationController : Controller
    {
        // GET: Operation
        public ActionResult FetchModelFiles(string project)
        {
            if (null != project)
            {
                using (var db = new IntegrateDbContext())
                {
                    string sql = string.Format(StaticSql.PROJECT_MODEL_LIST, project);
                    List<ModelFiles> modelFiles = db.Database.SqlQuery<ModelFiles>(sql).ToList();
                    return Json(modelFiles,JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new List<ModelFiles>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FetchScenes(int? projectId = 366)
        {
            try
            {
                using (var db = new IntegrateDbContext())
                {
                    string sql = string.Format(StaticSql.SCENES, projectId);
                    List<Sence> sences = db.Database.SqlQuery<Sence>(sql).ToList();
                    return Json(sences, JsonRequestBehavior.AllowGet);
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }
    }
}