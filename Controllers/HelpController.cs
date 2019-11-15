using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ConfigCenterApp.Controllers
{
    public class HelpController : Controller
    {
        // GET: Help
        public ActionResult SystemConfig()
        {
            return View();
        }

        public ActionResult ModelIntegrate()
        {
            return View();
        }

        public ActionResult SceneConfig()
        {
            return View();
        }
    }
}