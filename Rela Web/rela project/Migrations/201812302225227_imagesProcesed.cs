namespace Rela_project.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imagesProcesed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ImagesProceseds",
                c => new
                    {
                        imageId = c.Int(nullable: false, identity: true),
                        date = c.DateTime(nullable: false),
                        UserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.imageId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ImagesProceseds", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.ImagesProceseds", new[] { "UserId" });
            DropTable("dbo.ImagesProceseds");
        }
    }
}
