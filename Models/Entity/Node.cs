using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigCenterApp.Models.Entity
{
    public class Node
    {
        public int id { get; set; }
        public string text { get; set; }
        public List<Node> nodes { get; set; }
        public string icon { get; set; }
        public SystemConfig tags { get; set; }
        public string code { get; set; }
        public string parentCode { get; set; }
    }
}