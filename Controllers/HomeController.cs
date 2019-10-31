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
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Integrate()
        {
            using (var db = new IntegrateDbContext())
            {
                string projectListSql = string.Format(StaticSql.PROJECT_LIST);
                string floorsSql = StaticSql.FLOORS;
                string specialtiesSql = StaticSql.SPECIALTY;

                var projectList = db.Database.SqlQuery<Project>(projectListSql);
                var floorList = db.Database.SqlQuery<Floor>(floorsSql);
                var specialtyList = db.Database.SqlQuery<Specialty>(specialtiesSql);

                List<SelectListItem> projectListItems = new List<SelectListItem>();
                List<SelectListItem> floorsItems = new List<SelectListItem>();
                List<SelectListItem> specialtyItems = new List<SelectListItem>();
                if (null != projectList)
                {
                    List<Project> projects = projectList.ToList();
                    projects.ForEach(item =>
                    {
                        SelectListItem projectItem = new SelectListItem();
                        projectItem.Text = item.Name;
                        projectItem.Value = item.Id.ToString();
                        projectListItems.Add(projectItem);
                    });

                    floorList.Distinct().ToList().ForEach(item =>
                    {
                        SelectListItem floorItem = new SelectListItem();
                        floorItem.Text = item.FloorName;
                        floorItem.Value = item.FloorName;
                        floorsItems.Add(floorItem);
                    });

                    specialtyList.Distinct().ToList().ForEach(item =>
                    {
                        SelectListItem specialtyItem = new SelectListItem();
                        specialtyItem.Text = item.SpecialtyName;
                        specialtyItem.Value = item.SpecialtyName;
                        specialtyItems.Add(specialtyItem);
                    });
                }
                projectListItems.Insert(0, new SelectListItem { Text = "请选择项目", Value = "-1", Selected = true });
                ViewBag.SelectItems = projectListItems;
                ViewBag.floors = floorsItems;
                ViewBag.specialty = specialtyItems;
            }
            return View();
        }

        public ActionResult Authority()
        {
            using (var db = new IntegrateDbContext())
            {
                var projectListItems = new List<SelectListItem>();
                var projectListSql = string.Format(StaticSql.PROJECT_LIST);
                var projectList = db.Database.SqlQuery<Project>(projectListSql);
                List<Project> projects = projectList.ToList();
                projects.ForEach(item =>
                {
                    SelectListItem projectItem = new SelectListItem();
                    projectItem.Text = item.Name;
                    projectItem.Value = item.Id.ToString();
                    projectListItems.Add(projectItem);
                });
                projectListItems.Insert(0, new SelectListItem { Text = "请选择项目", Value = "-1", Selected = true });
                ViewBag.SelectItems = projectListItems;
            }
            return View();
        }

        public ActionResult Scene()
        {
            //TODO:
            List<string> sceneList = new List<string> { "设备管理", "报警管理" };
            ViewBag.Scene = JsonConvert.SerializeObject(sceneList);
            return View();
        }
    }
}