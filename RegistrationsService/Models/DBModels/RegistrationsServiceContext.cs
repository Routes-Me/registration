using Microsoft.EntityFrameworkCore;

namespace RegistrationsService.Models.DBModels
{
    public partial class RegistrationsServiceContext : DbContext
    {
        public RegistrationsServiceContext()
        {
        }

        public RegistrationsServiceContext(DbContextOptions<RegistrationsServiceContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
