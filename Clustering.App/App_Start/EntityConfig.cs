using System.Data.Entity;
using Clustering.Model;

namespace Clustering.App
{
    public class EntityConfig
    {
        public static void InitializeDataContext()
        {
            var initialCreate = new MigrateDatabaseToLatestVersion<ClusteringContext, Model.Seed.Configuration>();
            Database.SetInitializer<ClusteringContext>(initialCreate);
            using (var db = new ClusteringContext())
            {
                db.Database.Initialize(true);
            }
        }
    }
}