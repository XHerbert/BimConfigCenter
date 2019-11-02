using ConfigCenterApp.Models.Entity;
using IntegrateWebApp.Models.Database;
using IntegrateWebApp.Models.Entity;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ConfigCenterApp.Controllers
{
    public class HomeController : Controller
    {

        private List<Node> tree = new List<Node>();
        List<Node> nodes = new List<Node>();
        public ActionResult Index(int? projectId = 366)
        {
            using (var db = new IntegrateDbContext())
            {
                string sql = string.Format(StaticSql.SYSTEM_CONFIG, projectId);
                List<SystemConfig> systemConfigList = db.Database.SqlQuery<SystemConfig>(sql).ToList();

                // 构建节点
                systemConfigList.ForEach(item =>
                {
                    nodes.Add(new Node { id = item.id, text = item.sysName, nodes = new List<Node>(), tags = item, code = item.sysCode, parentCode = item.parentCode });
                });

                // 递归建立父子关系
                nodes.ForEach(o => buildTree(o));
                ViewBag.tree = JsonConvert.SerializeObject(tree);
            }
            return View();
        }

        private void buildTree(Node node)
        {
            if (tree.Contains(node))
            {
                return;
            }
            var pcode = node.parentCode;
            if (pcode == null)
            {
                tree.Add(node);
            }
            else
            {
                Node pnode = nodes.Where(item => item.code == pcode).FirstOrDefault();
                pnode.nodes.Add(node);
                buildTree(pnode);
            }
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
            using (var db = new IntegrateDbContext())
            {
                var projectListItems = new List<SelectListItem>();
                var projectListSql = string.Format(StaticSql.PROJECT_LIST);
                var projectList = db.Database.SqlQuery<Project>(projectListSql);
                List<Project> projects = projectList.ToList();
                IDictionary<int, string> projectDictionary = new Dictionary<int, string>();
                //projectDictionary.Add(-1, "- 请选择项目 -");
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