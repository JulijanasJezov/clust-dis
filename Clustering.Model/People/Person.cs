using Clustering.Model;
using System.Collections.Generic;
using System;

namespace Clustering.Model
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public virtual ICollection<PersonDiseasePropertyAssociation> PersonDiseaseProperties { get; set; }
    }
}