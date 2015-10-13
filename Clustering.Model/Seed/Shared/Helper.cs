using Clustering.Model.Seed.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.Model.Seed.Shared
{
    public static class Helper
    {
        public static string GetRandomMaleFirstName()
        {
            return StaticData.MaleFirstNames[ThreadSafeRandom.Next(0, StaticData.MaleFirstNames.Length - 1)];
        }

        public static string GetRandomFemaleFirstName()
        {
            return StaticData.FemaleFirstNames[ThreadSafeRandom.Next(0, StaticData.FemaleFirstNames.Length - 1)];
        }

        public static string GetRandomFamilyName()
        {
            return StaticData.FamilyNames[ThreadSafeRandom.Next(0, StaticData.FamilyNames.Length - 1)];
        }

        public static DateTime GenerateDateOfBirth()
        {
            const int oldest = 80;
            const int youngest = 12;

            var earliestDate = DateTime.Now.Subtract(TimeSpan.FromDays(365 * oldest));
            var latestDate = DateTime.Now.Subtract(TimeSpan.FromDays(365 * youngest));

            int range = (latestDate - earliestDate).Days;
            var dateOfBirth = earliestDate.AddDays(ThreadSafeRandom.Next(0, range));

            return dateOfBirth.Date;
        }
    }
}
