using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace POS.Models;

public partial class PosAbarrotesContext : DbContext
{
    public PosAbarrotesContext()
    {
    }

    public PosAbarrotesContext(DbContextOptions<PosAbarrotesContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AperturaCaja> AperturaCajas { get; set; }

    public virtual DbSet<Categoria> Categorias { get; set; }

    public virtual DbSet<CierreCajas> CierreCajas { get; set; }

    public virtual DbSet<DetallesVentum> DetallesVenta { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<ReporteVenta> ReporteVentas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-OLORG47G;Database=POS_Abarrotes;User Id=sa;Password=root;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AperturaCaja>(entity =>
        {
            entity.HasKey(e => e.IdApertura).HasName("PK__Apertura__23205EF8D7778EAA");

            entity.ToTable("AperturaCaja");

            entity.Property(e => e.IdApertura).HasColumnName("idApertura");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.MontoInicial)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoInicial");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.AperturaCajas)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__AperturaC__idUsu__4D94879B");
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.IdCategoria).HasName("PK__Categori__8A3D240C63EBD0FE");

            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.NombreCategoria)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreCategoria");
        });

        modelBuilder.Entity<CierreCajas>(entity =>
        {
            entity.HasKey(e => e.IdCierre).HasName("PK__CierreCa__51DD08790E571B0E");

            entity.ToTable("CierreCajas");

            entity.Property(e => e.IdCierre).HasColumnName("idCierre");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdApertura).HasColumnName("idApertura");
            entity.Property(e => e.MontoFinal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoFinal");

            entity.HasOne(d => d.IdAperturaNavigation).WithMany(p => p.CierreCajas)
                .HasForeignKey(d => d.IdApertura)
                .HasConstraintName("FK__CierreCaj__idApe__5070F446");
        });

        modelBuilder.Entity<DetallesVentum>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__Detalles__49CAE2FBA7BE5B1E");

            entity.ToTable(tb => tb.HasTrigger("tr_ActualizarStock"));

            entity.Property(e => e.IdDetalle).HasColumnName("idDetalle");
            entity.Property(e => e.Cantidad)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("cantidad");
            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.IdVenta).HasColumnName("idVenta");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioUnitario");
            entity.Property(e => e.Subtotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__DetallesV__idPro__49C3F6B7");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetallesVenta)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK__DetallesV__idVen__48CFD27E");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__Producto__07F4A132A888084C");

            entity.HasIndex(e => e.CodigoBarras, "UQ__Producto__42674CE2BA75AAB3").IsUnique();

            entity.Property(e => e.IdProducto).HasColumnName("idProducto");
            entity.Property(e => e.CodigoBarras)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("codigoBarras");
            entity.Property(e => e.IdCategoria).HasColumnName("idCategoria");
            entity.Property(e => e.NombreProducto)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreProducto");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.Stock)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("stock");

            entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdCategoria)
                .HasConstraintName("FK__Productos__idCat__412EB0B6");
        });

        modelBuilder.Entity<ReporteVenta>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ReporteVentas");

            entity.Property(e => e.Cajero)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdVenta).HasColumnName("idVenta");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("metodoPago");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__3C872F769A02BC9C");

            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.NombreRol)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreRol");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuarios__645723A6F0D3F448");

            entity.HasIndex(e => e.Usuario1, "UQ__Usuarios__9AFF8FC6964321FE").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.Contrasena)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("contrasena");
            entity.Property(e => e.IdRol).HasColumnName("idRol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("usuario");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuarios__idRol__3A81B327");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Ventas__077D56145D56D06A");

            entity.Property(e => e.IdVenta).HasColumnName("idVenta");
            entity.Property(e => e.Fecha)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("metodoPago");
            entity.Property(e => e.Total)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__Ventas__idUsuari__44FF419A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
