using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Clustering.App.Api.Shared.Algorithms;

namespace Clustering.App.Api
{
    public class ClusterApiModel
    {
        public string Name { get; set; }
        public List<KMDataPoint> DataPoints { get; set; }
        public List<PropertyDetails> PropertiesDetails { get; set; }
        public int Total { get; set; }
    }
}