using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.Model.Seed.Data
{
    public class DataDiseaseProperty
    {
        public string Name { get; set; }
        public string Disease { get; set; }
    }

    public static partial class StaticData
    {
        public static List<DataDiseaseProperty> DiseaseProperties = new List<DataDiseaseProperty>()
        {
            new DataDiseaseProperty { Name = "Happiness", Disease = "Depression" },
            new DataDiseaseProperty { Name = "Energy", Disease = "Depression" },
            new DataDiseaseProperty { Name = "DepressionLevel", Disease = "Depression" }
        };
    }
}
