using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clustering.App.Api.Models
{
    public class PersonApiModel
    {
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; }
        public List<PersonDiseaseProperty> DiseaseProperties { get; set; }
        public List<int> RemoveDiseases { get; set; }
    }
}