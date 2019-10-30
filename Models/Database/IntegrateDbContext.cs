using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace IntegrateWebApp.Models.Database
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class IntegrateDbContext : DbContext
    {
        public IntegrateDbContext()
            : base("connection")
        {

        }       
    }
}