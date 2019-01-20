namespace GestionDeTurnos.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prueba : DbMigration
    {
        public override void Up()
        {

        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PersonMedicals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PersonId = c.Int(nullable: false),
                        Avoi = c.Int(nullable: false),
                        Avod = c.Int(nullable: false),
                        Fuma = c.Boolean(nullable: false),
                        Profesional = c.Boolean(nullable: false),
                        ConduceConAnteojos = c.Boolean(nullable: false),
                        VisionMonocular = c.Boolean(nullable: false),
                        Discromatopsia = c.Boolean(nullable: false),
                        HTA = c.Boolean(nullable: false),
                        DBT = c.Boolean(nullable: false),
                        GAA = c.Boolean(nullable: false),
                        AcidoUrico = c.Boolean(nullable: false),
                        Colesterol = c.Boolean(nullable: false),
                        Observacion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.PersonMedicals", "PersonId");
            AddForeignKey("dbo.PersonMedicals", "PersonId", "dbo.People", "Id");
        }
    }
}
