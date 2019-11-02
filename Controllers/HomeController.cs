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

        private List<SystemConfigNode> systemConfigTree = new List<SystemConfigNode>();
        private List<DataTransferNode> dataTransferTree = new List<DataTransferNode>();
        List<SystemConfigNode> systemConfigNodes = new List<SystemConfigNode>();
        List<DataTransferNode> dataTransferNodes = new List<DataTransferNode>();
        public ActionResult Index(int? projectId = 366)
        {
            using (var db = new IntegrateDbContext())
            {

                var project = string.Format(StaticSql.PROJECT, projectId);
                var projectEntity = db.Database.SqlQuery<Project>(project).FirstOrDefault();
                var tplId = projectEntity.TplId;

                // 获取数字化移交设备分类
                var fileSql = string.Format(StaticSql.TPL_FILE, tplId, projectId);
                List<File> files = db.Database.SqlQuery<File>(fileSql).ToList();

                var sql = string.Format(StaticSql.SYSTEM_CONFIG, projectId);
                List<SystemConfig> systemConfigList = db.Database.SqlQuery<SystemConfig>(sql).ToList();

                files.ForEach(file =>
                {
                    dataTransferNodes.Add(
                        new DataTransferNode
                        {
                            id = file.Id,
                            text = file.Name,
                            nodes = new List<DataTransferNode>(),
                            tags = file,
                            code = file.Code,
                            parentCode = file.ParentId
                        });
                });
                dataTransferNodes.ForEach(o => buildDataTransferTree(o));
                ViewBag.dataTransferTree = JsonConvert.SerializeObject(dataTransferTree);

                // 构建系统分类节点
                systemConfigList.ForEach(item =>
                {
                    systemConfigNodes.Add(
                        new SystemConfigNode
                        {
                            id = item.id,
                            text = item.sysName,
                            nodes = new List<SystemConfigNode>(),
                            tags = item,
                            code = item.sysCode,
                            parentCode = item.parentCode
                        });
                });

                // 递归建立父子关系
                systemConfigNodes.ForEach(o => buildSystemConfigTree(o));
                ViewBag.systemConfigTree = JsonConvert.SerializeObject(systemConfigTree);
            }
            return View();
        }

        private void buildSystemConfigTree(SystemConfigNode node)
        {
            if (systemConfigTree.Contains(node))
            {
                return;
            }
            var pcode = node.parentCode;
            if (pcode == null)
            {
                systemConfigTree.Add(node);
            }
            else
            {
                SystemConfigNode pnode = systemConfigNodes.Where(item => item.code == pcode).FirstOrDefault();
                pnode.nodes.Add(node);
                buildSystemConfigTree(pnode);
            }
        }

        private void buildDataTransferTree(DataTransferNode node)
        {
            if (dataTransferTree.Contains(node))
            {
                return;
            }
            var pcode = node.parentCode;
            if (pcode == null)
            {
                dataTransferTree.Add(node);
            }
            else
            {
                DataTransferNode pnode = dataTransferNodes.Where(item => item.id == pcode).FirstOrDefault();
                pnode.nodes.Add(node);
                buildDataTransferTree(pnode);
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