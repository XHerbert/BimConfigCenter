using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigCenterApp.Models.Entity
{
    public class Sence
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string MergeName { get; set; }
        public string MergeMemo { get; set; }
        public string FunctionCode { get; set; }
        public long IntegrateId { get; set; }
        public DateTime MergeTime { get; set; }
        public string Merger { get; set; }
        public string BackPicture { get; set; }
        public string ShowSet { get; set; }
        public bool BorderLine { get; set; }
        public bool ShowSun { get; set; }
        public string Camera { get; set; }
        public string DefineAPI { get; set; }
    }
}