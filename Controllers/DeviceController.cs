using IntegrateWebApp.Models.Database;
using IntegrateWebApp.Models.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConfigCenterApp.Controllers
{
    public class DeviceController : Controller
    {
        // GET: Device
        public ActionResult DeviceGroup()
        {
            return View();
        }

        public ActionResult ConvertReferenceView()
        {
            return View();
        }

        public ActionResult ConvertReferenceJson(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return Content(string.Empty);
            }

            string _json = json.Replace("\"Id\":", "");
            _json = _json.Replace("{", "").Replace("}", "");
            return Content(_json.ToString());
        }

        public ActionResult GetThresholdList()
        {
            using (var db = new IntegrateDbContext())
            {
                var projectListItems = new List<SelectListItem>();
                var projectListSql = string.Format(StaticSql.PROJECT_LIST);
                var projectList = db.Database.SqlQuery<Project>(projectListSql);
                List<Project> projects = projectList.ToList();
                IDictionary<int, string> projectDictionary = new Dictionary<int, string>();
                projects.ForEach(item =>
                {
                    SelectListItem projectItem = new SelectListItem();
                    projectItem.Text = item.Name;
                    projectItem.Value = item.Id.ToString();
                    projectListItems.Add(projectItem);

                    projectDictionary.Add(item.Id, item.Name);
                });
                projectListItems.Insert(0, new SelectListItem { Text = "请选择项目", Value = "-1", Selected = true });
                ViewBag.SelectItems = projectListItems;
                ViewBag.Projects = JsonConvert.SerializeObject(projects);
            }
            return View();
        }
    }
}