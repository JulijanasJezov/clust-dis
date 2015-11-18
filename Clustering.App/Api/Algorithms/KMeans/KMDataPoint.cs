using System.Collections.Generic;

namespace Clustering.App.Api.Algorithms
{
    public class KMDataPoint
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IDictionary<string, double> Properties { get; set; }
        public int Cluster { get; set; }
        public double Mean { get; set; }
        public double StandardDeviation { get; set; }
        public double Silhouette { get; set; }

        public KMDataPoint(IDictionary<string, double> properties, string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Properties = properties;
            Cluster = 0;
        }

        public KMDataPoint(IDictionary<string, double> properties)
        {
            Properties = properties;
            Cluster = 0;
        }

        public KMDataPoint()
        {

        }
    }
}