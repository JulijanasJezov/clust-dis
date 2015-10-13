using Clustering.Model.Seed.Data;
using Clustering.Model.Seed.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Clustering.Model.Seed.Tasks
{
    public static partial class SeedTasks
    {
        public static Task SeedPeople()
        {
            var task = new Task(() =>
            {
                const int PeopleToSeed = 2000;

                Parallel.For(0, PeopleToSeed, i =>
                {
                    using (var context = new ClusteringContext())
                    {
                        var isMale = ThreadSafeRandom.Next(0, 10) > 5;

                        var firstName = isMale ?
                            Helper.GetRandomMaleFirstName() :
                            Helper.GetRandomFemaleFirstName();

                        var lastName = Helper.GetRandomFamilyName();

                        var title = "";
                        if (isMale)
                        {
                            title = StaticData.Titles
                                .Where(t => t.Gender == "M" || t.Gender == "U")
                                .Select(s => s.Name)
                                .OrderBy(s => Guid.NewGuid())
                                .First();
                        }
                        else
                        {
                            title = StaticData.Titles
                                .Where(t => t.Gender == "F" || t.Gender == "U")
                                .Select(s => s.Name)
                                .OrderBy(s => Guid.NewGuid())
                                .First();
                        }

                        var dateOfBirth = Helper.GenerateDateOfBirth();

                        var depressionLevel = ThreadSafeRandom.Next(0, 50); // Temp ex

                        var person = new Person
                        {
                            Title = title,
                            FirstName = firstName,
                            LastName = lastName,
                            Gender = isMale ? GenderType.Male : GenderType.Female,
                            DateOfBirth = dateOfBirth,
                            DepressionLevel = depressionLevel
                        };

                        context.People.Add(person);

                        context.SaveChanges();
                    }
                });
            });

            task.Start();
            return task;
        }
    }
}