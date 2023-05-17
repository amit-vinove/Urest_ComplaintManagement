using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using UrestComplaintManagement.Context;

namespace UrestComplaintManagement
{
    public class DBContext : DbContext
    {
        public DBContext()
            : base("Name=adoConnectionstring")
        {
            Database.SetInitializer<DBContext>(new DropCreateDatabaseIfModelChanges<DBContext>());
        }
        public DbSet<KycImages> KycImages { get; set; }
    }
}