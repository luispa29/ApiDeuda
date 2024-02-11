using System;
using System.Collections.Generic;
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

    public virtual DbSet<Abono> Abonos { get; set; }

    public virtual DbSet<Deudore> Deudores { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Abono>(entity =>
        {
            entity.Property(e => e.Abono1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Abono");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.Abonos)
                .HasForeignKey(d => d.IdPrestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Abonos_prestamo");
        });

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

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.ImagenId)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ImagenUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MontoPrestamo).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdDeudorNavigation).WithMany(p => p.InverseIdDeudorNavigation)
                .HasForeignKey(d => d.IdDeudor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_deudor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_usuario");
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
