using Clustering.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api.Models
{
    public class DiseaseApiModel
    {
        public int DiseaseId { get; set; }
        public string Name { get; set; }
        public List<DiseaseProperty> DiseaseProperties { get; set; }
        public bool CanDelete { get; set; }
    }
}