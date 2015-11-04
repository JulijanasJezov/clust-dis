using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api
{
    public class PropertyDetails
    {
        public string Name { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public double AverageValue { get; set; }
    }
}