using Clustering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api
{
    public class PostClusterApiModel
    {
        public int ClusterDiseaseId { get; set; }
        public int[] ClusterPropertyIds { get; set; }
        public bool CalculateSilhouette { get; set; }
        public bool IncludeAge { get; set; }
    }
}