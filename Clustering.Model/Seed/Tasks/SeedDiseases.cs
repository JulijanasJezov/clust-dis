using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.Model.Seed.Data;
using Clustering.Model.Seed.Shared;

namespace Clustering.Model.Seed.Tasks
{
    public static partial class SeedTasks
    {
        public static Task SeedDiseases()
        {
            var task = new Task(() =>
            {
                foreach (var disease in StaticData.Diseases)
                {
                    using (var context = new ClusteringContext())
                    {
                        var diseaseToImport = new Disease
                        {
                            Name = disease
                        };

                        context.Diseases.Add(diseaseToImport);

                        context.SaveChanges();
                    }
                }
                });

            task.Start();
            return task;
        }
    }
}
