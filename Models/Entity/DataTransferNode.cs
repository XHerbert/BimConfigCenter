using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigCenterApp.Models.Entity
{
    public class DataTransferNode : Node
    {
        public List<DataTransferNode> nodes { get; set; }
        public File tags { get; set; }
        public int? parentCode { get; set; }
    }
}