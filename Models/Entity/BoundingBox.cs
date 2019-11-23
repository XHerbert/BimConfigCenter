using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConfigCenterApp.Models.Entity
{
    public class BoundingBox
    {
        public Coordinate min { get; set; }
        public Coordinate max { get; set; }
    }
}