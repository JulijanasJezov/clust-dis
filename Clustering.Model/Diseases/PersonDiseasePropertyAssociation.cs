using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.Model
{
    public class PersonDiseasePropertyAssociation
    {
        [ForeignKey("Person")]
        [Key, Column(Order = 0)]
        public int PersonId { get; set; }
        public virtual Person Person { get; set; }

        [ForeignKey("DiseaseProperty")]
        [Key, Column(Order = 1)]
        public int DiseasePropertyId { get; set; }
        public virtual DiseaseProperty DiseaseProperty { get; set; }

        public int Score { get; set; }
    }
}
