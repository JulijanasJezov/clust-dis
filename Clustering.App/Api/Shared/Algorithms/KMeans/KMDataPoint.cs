﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api.Shared.Algorithms
{
    public class KMDataPoint
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IDictionary<string, double> Properties { get; set; }
        public int Cluster { get; set; }

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