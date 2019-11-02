using System.Collections.Generic;

namespace ConfigCenterApp.Models.Entity
{
    public class SystemConfigNode : Node
    {
        public List<SystemConfigNode> nodes { get; set; }
        public SystemConfig tags { get; set; }
        public string parentCode { get; set; }
    }
}