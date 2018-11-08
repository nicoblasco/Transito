
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GestionDeTurnos.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {

        }


        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Audit> Audits { get; set; }


        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Module> Modules { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Permission> Permissions { get; set; }


        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Rol> Rols { get; set; }



        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Usuario> Usuarios { get; set; }


        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Window> Windows { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Act>()
            //.HasMany(c => c.)
            //.WithOptional()
            //.Map(m => m.MapKey("ClaimId"));

        }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.TypesLicense> TypesLicenses { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Sector> Sectors { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Workflow> Workflows { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.SectorWorkflow> SectorWorkflows { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Turn> Turns { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Person> People { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Terminal> Terminals { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.CallCenterTurn> CallCenterTurns { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Setting> Settings { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Tracking> Trackings { get; set; }

        public System.Data.Entity.DbSet<GestionDeTurnos.Models.Status> Status { get; set; }
    }
}