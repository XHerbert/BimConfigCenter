using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigCenterApp.Models.Entity
{
    public class File
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
        public int? Seq { get; set; }
    }
}