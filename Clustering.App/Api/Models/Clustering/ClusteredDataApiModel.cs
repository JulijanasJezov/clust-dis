using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Clustering.App.Api.Algorithms;

namespace Clustering.App.Api
{
    public class ClusteredDataApiModel
    {
        public string Name { get; set; }
        public List<KMDataPoint> DataPoints { get; set; }
        public List<PropertyDetails> PropertiesDetails { get; set; }
        public int Total { get; set; }
    }
}