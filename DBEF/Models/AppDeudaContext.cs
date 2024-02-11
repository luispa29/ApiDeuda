
using Microsoft.EntityFrameworkCore;

namespace DBEF.Models;

public partial class AppDeudaContext : DbContext
{
    public AppDeudaContext()
    {
    }

    public AppDeudaContext(DbContextOptions<AppDeudaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Usuario> Usuarios { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.Admin).HasColumnName("admin");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
