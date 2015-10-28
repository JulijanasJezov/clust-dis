namespace Clustering.Model.Seed
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExpandableDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Diseases",
                c => new
                    {
                        DiseaseId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.DiseaseId);
            
            CreateTable(
                "dbo.DiseaseProperties",
                c => new
                    {
                        DiseasePropertyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DiseaseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DiseasePropertyId)
                .ForeignKey("dbo.Diseases", t => t.DiseaseId)
                .Index(t => t.DiseaseId);
            
            CreateTable(
                "dbo.PersonDiseasePropertyAssociations",
                c => new
                    {
                        PersonId = c.Int(nullable: false),
                        DiseasePropertyId = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PersonId, t.DiseasePropertyId })
                .ForeignKey("dbo.DiseaseProperties", t => t.DiseasePropertyId)
                .ForeignKey("dbo.People", t => t.PersonId)
                .Index(t => t.PersonId)
                .Index(t => t.DiseasePropertyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PersonDiseasePropertyAssociations", "PersonId", "dbo.People");
            DropForeignKey("dbo.PersonDiseasePropertyAssociations", "DiseasePropertyId", "dbo.DiseaseProperties");
            DropForeignKey("dbo.DiseaseProperties", "DiseaseId", "dbo.Diseases");
            DropIndex("dbo.PersonDiseasePropertyAssociations", new[] { "DiseasePropertyId" });
            DropIndex("dbo.PersonDiseasePropertyAssociations", new[] { "PersonId" });
            DropIndex("dbo.DiseaseProperties", new[] { "DiseaseId" });
            DropTable("dbo.PersonDiseasePropertyAssociations");
            DropTable("dbo.DiseaseProperties");
            DropTable("dbo.Diseases");
        }
    }
}
