using Clustering.Model.Seed.Tasks;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Clustering.Model.Seed
{
    public sealed class Configuration : DbMigrationsConfiguration<ClusteringContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Clustering.Model.ClusteringContext";
        }
    }
}