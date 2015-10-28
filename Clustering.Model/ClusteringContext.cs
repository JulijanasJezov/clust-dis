using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Clustering.Model
{
    public class ClusteringContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Disease> Diseases { get; set; }
        public DbSet<DiseaseProperty> DiseaseProperties { get; set; }
        public DbSet<PersonDiseasePropertyAssociation> PersonDiseasePropertyAssociations { get; set; }

        public ClusteringContext()
            : base("Clustering")
        {
            var commandTimeout = 60;
            if (ConfigurationManager.AppSettings["CommandTimeout"] != null)
            {
                commandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeout"]);
            }

            Database.CommandTimeout = commandTimeout;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToOneConstraintIntroductionConvention>();
        }
    }
}