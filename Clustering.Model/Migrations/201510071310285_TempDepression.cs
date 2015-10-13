namespace Clustering.Model.Seed
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TempDepression : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "DepressionLevel", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "DepressionLevel");
        }
    }
}
