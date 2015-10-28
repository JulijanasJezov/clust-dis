using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.Model
{
    public class DiseaseProperty
    {
        public int DiseasePropertyId { get; set; }
        public string Name { get; set; }

        [ForeignKey("Disease")]
        public int DiseaseId { get; set; }
        public virtual Disease Disease { get; set; }
    }
}
