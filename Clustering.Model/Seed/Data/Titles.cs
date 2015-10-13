using System.Collections.Generic;

namespace Clustering.Model.Seed.Data
{
    public class DataTitle
    {
        public string Name { get; set; }
        public string Gender { get; set; }
    }

    public static partial class StaticData
    {
        public static List<DataTitle> Titles = new List<DataTitle>()
        {
            new DataTitle { Name = "Mr", Gender = "M" },
            new DataTitle { Name = "Mrs", Gender = "F" },
            new DataTitle { Name = "Miss", Gender = "F" },
            new DataTitle { Name = "Ms", Gender = "F" },
            new DataTitle { Name = "Dr", Gender = "U" },
            new DataTitle { Name = "Lord", Gender = "M" },
            new DataTitle { Name = "Rev", Gender = "M" },
            new DataTitle { Name = "Dame", Gender = "F" },
            new DataTitle { Name = "Sir", Gender = "M" },
            new DataTitle { Name = "Prof", Gender = "U" },
            new DataTitle { Name = "Lady", Gender = "F" }
        };
    }
}
