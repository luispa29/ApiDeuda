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

    public virtual DbSet<Catalogo> Catalogos { get; set; }

    public virtual DbSet<Deudore> Deudores { get; set; }

    public virtual DbSet<Ignorado> Ignorados { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<Presupuesto> Presupuestos { get; set; }

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

        modelBuilder.Entity<Catalogo>(entity =>
        {
            entity.ToTable("Catalogo");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Categoria)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
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

        modelBuilder.Entity<Ignorado>(entity =>
        {
            entity.HasOne(d => d.IdDeudorNavigation).WithMany(p => p.Ignorados)
                .HasForeignKey(d => d.IdDeudor)
                .HasConstraintName("FK_Ignorados_deudor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Ignorados)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Ignorados_Usuario");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.TimeStamp).HasColumnType("datetime");
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
            entity.Property(e => e.Propio).HasColumnName("propio");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.PrestamoIdCategoriaNavigations)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK_Prestamos_Categoria");

            entity.HasOne(d => d.IdDeudorNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.IdDeudor)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_deudor");

            entity.HasOne(d => d.IdMedioNavigation).WithMany(p => p.PrestamoIdMedioNavigations)
                .HasForeignKey(d => d.IdMedio)
                .HasConstraintName("FK_Prestamos_Medio");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Prestamos_usuario");
        });

        modelBuilder.Entity<Presupuesto>(entity =>
        {
            entity.Property(e => e.Presupuesto1)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Presupuesto");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Presupuestos)
                .HasForeignKey(d => d.UsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Presupuestos_Usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(e => e.Admin).HasColumnName("admin");
            entity.Property(e => e.CodigoCompartido)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("codigoCompartido");
            entity.Property(e => e.Correo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("correo");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
