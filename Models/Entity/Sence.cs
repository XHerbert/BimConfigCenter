using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigCenterApp.Models.Entity
{
    public class Sence
    {
        public int sceneId { get; set; }
        public int projectId { get; set; }
        public string mergeName { get; set; }
        public string mergeMemo { get; set; }
        public string funcCode { get; set; }
        public long integrateId { get; set; }
        //public DateTime mergeTime { get; set; }
        public string merger { get; set; }
        public string backPic { get; set; }
        public string showSet { get; set; }
        public bool borderLine { get; set; }
        public bool showSun { get; set; }
        public string camera { get; set; }
        public string defApi { get; set; }
    }
}