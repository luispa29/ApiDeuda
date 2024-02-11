using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Api.Models;

public partial class AppDeudaContext : DbContext
{
    public AppDeudaContext()
    {
    }

    public AppDeudaContext(DbContextOptions<AppDeudaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Deudore> Deudores { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deudore>(entity =>
        {
            entity.Property(e => e.Nombres)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Deudores)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Deudores_Usuarios");
        });

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
