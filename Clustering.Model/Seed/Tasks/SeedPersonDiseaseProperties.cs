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
        public static Task SeedPersonDiseaseProperties()
        {
            var task = new Task(() =>
            {
                using (var context = new ClusteringContext())
                {
                    var associations = new List<PersonDiseasePropertyAssociation>();

                    foreach (var person in context.People)
                    {
                        foreach (var diseaseProperty in context.DiseaseProperties)
                        {
                            int maxScore = 0;
                            if (person.DateOfBirth > new DateTime(1990, 1, 1)) maxScore = 20;
                            if (person.DateOfBirth > new DateTime(1960, 1, 1) && person.DateOfBirth < new DateTime(1990, 1, 1)) maxScore = 35;
                            if (person.DateOfBirth > new DateTime(1900, 1, 1) && person.DateOfBirth < new DateTime(1960, 1, 1)) maxScore = 50;

                            var score = ThreadSafeRandom.Next(0, maxScore);

                            var personDiseaseProperty = new PersonDiseasePropertyAssociation
                            {
                                PersonId = person.PersonId,
                                DiseasePropertyId = diseaseProperty.DiseasePropertyId,
                                Score = score
                            };

                            associations.Add(personDiseaseProperty);
                        }
                    }

                    context.PersonDiseasePropertyAssociations.AddRange(associations);
                    context.SaveChanges();
                }
            });

            task.Start();
            return task;
        }
    }
}
