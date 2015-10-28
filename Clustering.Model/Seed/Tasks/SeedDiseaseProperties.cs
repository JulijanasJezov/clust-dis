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
        public static Task SeedDiseaseProperties()
        {
            var task = new Task(() =>
            {
                using (var context = new ClusteringContext())
                {
                    foreach (var disease in context.Diseases)
                    {
                        foreach (var diseaseProperty in StaticData.DiseaseProperties.Where(s => s.Disease == disease.Name))
                        {
                            var diseasePropertyToImport = new DiseaseProperty
                            {
                                Name = diseaseProperty.Name,
                                DiseaseId = disease.DiseaseId
                            };

                            context.DiseaseProperties.Add(diseasePropertyToImport);

                            
                        }
                    }

                    context.SaveChanges();
                }
            });

            task.Start();
            return task;
        }
    }
}
