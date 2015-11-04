using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api
{
    public class PostDiseaseApiModel
    {
        public string DiseaseName { get; set; }
        public List<PropertyDetails> Properties { get; set; }
    }
}