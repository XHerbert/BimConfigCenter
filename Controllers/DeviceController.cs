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
    }
}