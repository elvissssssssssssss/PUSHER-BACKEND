
    using global::apitextil.Models;
     
    using Microsoft.EntityFrameworkCore;
using apitextilECommerceAPI.Models;


namespace apitextil.Data
    {
    public partial class EcommerceContext : DbContext
    {

        public EcommerceContext(DbContextOptions<EcommerceContext> options)
      : base(options)
        {
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<TblEnvios> TblEnvios { get; set; }
        public DbSet<Tblusuario> tblusuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        // ✅ Correcto
        // ✅ Correcto
        public DbSet<MetodoPago> MetodoPagos { get; set; }
        public DbSet<TblAtencionCliente> TblAtencionCliente { get; set; }
        public DbSet<TblMensajesChat> TblMensajesChat { get; set; }




        public DbSet<Venta> Venta { get; set; }
        public DbSet<ComprobanteVenta> ComprobantesVenta { get; set; }

        // ✅ NUEVOS DbSets para el sistema de seguimiento
        public DbSet<EstadoEnvio> EstadoEnvios { get; set; }
        public DbSet<SeguimientoEnvio> SeguimientoEnvios { get; set; }
        public DbSet<DocumentoEnvio> DocumentoEnvios { get; set; }






        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {





                modelBuilder.Entity<Tblusuario>(entity =>
                {
                    entity.HasKey(e => e.id);
                    entity.Property(e => e.id).ValueGeneratedOnAdd();
                    entity.HasIndex(e => e.email).IsUnique();
                    entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
                    entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
                });

                // Configuración para la tabla productos
                modelBuilder.Entity<Producto>(entity =>
                {
                    entity.ToTable("productos");

                    entity.HasKey(e => e.Id);

                    entity.Property(e => e.Id)
                        .HasColumnName("id")
                        .ValueGeneratedOnAdd();

                    entity.Property(e => e.Nombre)
                        .HasColumnName("nombre")
                        .HasMaxLength(255)
                        .IsRequired();

                    entity.Property(e => e.Description)
                        .HasColumnName("description");

                    entity.Property(e => e.Marca)
                        .HasColumnName("marca")
                        .HasMaxLength(255)
                        .IsRequired();

                    entity.Property(e => e.Color)
                        .HasColumnName("color")
                        .HasMaxLength(255)
                        .IsRequired();

                    entity.Property(e => e.Precio)
                        .HasColumnName("precio")
                        .HasColumnType("decimal(10,2)")
                        .IsRequired();

                    entity.Property(e => e.PrecioDescuento)
                        .HasColumnName("precio_descuento")
                        .HasColumnType("decimal(10,2)");

                    entity.Property(e => e.Stock)
                        .HasColumnName("stock")
                        .HasDefaultValue(0);

                    entity.Property(e => e.Talla)
                        .HasColumnName("talla")
                        .HasMaxLength(10)
                        .IsRequired();

                    entity.Property(e => e.Code)
                        .HasColumnName("code")
                        .HasMaxLength(255)
                        .IsRequired();

                    entity.Property(e => e.imagen)
                        .HasColumnName("imagen")
                        .HasMaxLength(255)
                        .IsRequired();

                    entity.Property(e => e.imagen2)
                        .HasColumnName("imagen2")
                        .HasMaxLength(255);

                    entity.Property(e => e.imagen3)
                        .HasColumnName("imagen3")
                        .HasMaxLength(255);

                    entity.Property(e => e.Categoria)
                        .HasColumnName("categoria")
                        .HasMaxLength(255);

                    entity.Property(e => e.CreatedAt)
                        .HasColumnName("created_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");

                    entity.Property(e => e.UpdatedAt)
                        .HasColumnName("updated_at")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                    // Índices para mejorar el rendimiento
                    entity.HasIndex(e => e.Code)
                        .IsUnique()
                        .HasDatabaseName("idx_productos_code");

                    entity.HasIndex(e => e.Categoria)
                        .HasDatabaseName("idx_productos_categoria");

                    entity.HasIndex(e => e.Marca)
                        .HasDatabaseName("idx_productos_marca");
                });

           

            // Configuración para CartItem
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("cart");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20)")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("int(11)")
                    .IsRequired();

                entity.Property(e => e.ProductId)
                    .HasColumnName("product_id")
                    .HasColumnType("int(11)")
                    .IsRequired();

                entity.Property(e => e.Talla)
                    .HasColumnName("talla")
                    .HasColumnType("varchar(10)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci")
                    .IsRequired();

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("int(11)")
                    .HasDefaultValue(1);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Índices
                entity.HasIndex(e => e.UserId, "idx_user_id");
                entity.HasIndex(e => e.ProductId, "idx_product_id");
                entity.HasIndex(e => new { e.UserId, e.ProductId, e.Talla }, "unique_user_product_talla")
                    .IsUnique();

                // Relaciones (ajustar según tus modelos)
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("fk_cart_user");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("fk_cart_product");
            });
            // Configuración para TblEnvios
            modelBuilder.Entity<TblEnvios>(entity =>
            {
                entity.ToTable("tblenvios");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Direccion)
                    .HasColumnName("direccion")
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Provincia)
                    .HasColumnName("provincia")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Localidad)
                    .HasColumnName("localidad")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Dni)
                    .HasColumnName("dni")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.Telefono)
                    .HasColumnName("telefono")
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Índices para optimizar consultas
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_user_id");

                entity.HasIndex(e => e.Dni)
                    .HasDatabaseName("idx_dni");
            });
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.ToTable("tblventa");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.MetodoPagoId).HasColumnName("metodo_pago_id");

                entity.Property(e => e.Total)
                      .HasColumnName("total")
                      .HasColumnType("decimal(10,2)");

                entity.Property(e => e.FechaVenta)
                      .HasColumnName("fecha_venta")
                      .HasColumnType("timestamp")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasColumnType("timestamp")
                      .HasDefaultValue((DateTime?)null);

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at")
                      .HasColumnType("timestamp")
                      .HasDefaultValue((DateTime?)null)
                      .ValueGeneratedOnAddOrUpdate();

                // Relaciones
                entity.HasOne(e => e.User)
                      .WithMany()
                      .HasForeignKey(e => e.UserId)
                      .HasConstraintName("fk_venta_user");

                entity.HasOne(e => e.MetodoPago)
                      .WithMany()
                      .HasForeignKey(e => e.MetodoPagoId)
                      .HasConstraintName("fk_venta_metodopago");

                entity.HasMany(e => e.Detalles)
                      .WithOne(d => d.Venta)
                      .HasForeignKey(d => d.VentaId)
                      .HasConstraintName("fk_detalleventa_venta");
            });



            modelBuilder.Entity<ComprobanteVenta>(entity =>
            {
                entity.ToTable("tblcomprobanteventa");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.VentaId).HasColumnName("venta_id");
                entity.Property(e => e.TipoComprobante).HasColumnName("tipo_comprobante");
                entity.Property(e => e.Serie).HasColumnName("serie").HasMaxLength(10);
                entity.Property(e => e.Numero).HasColumnName("numero");
                entity.Property(e => e.EnlacePdf).HasColumnName("enlace_pdf").HasMaxLength(500);
                entity.Property(e => e.CodigoHash).HasColumnName("codigo_hash").HasMaxLength(100);
                entity.Property(e => e.CodigoQr).HasColumnName("codigo_qr").HasMaxLength(500);
                entity.Property(e => e.FechaGeneracion)
                      .HasColumnName("fecha_generacion")
                      .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // 🔗 Relación con tblventa
                entity.HasOne(e => e.Venta)
                      .WithOne(v => v.Comprobante)
                      .HasForeignKey<ComprobanteVenta>(e => e.VentaId)
                      .HasConstraintName("fk_comprobante_venta")
                      .OnDelete(DeleteBehavior.Cascade);
            });



            modelBuilder.Entity<MetodoPago>(entity =>
            {
                entity.ToTable("tblmetodopago");
                entity.HasKey(e => e.Id);
            });


            modelBuilder.Entity<Tblusuario>(entity =>
            {
                entity.HasKey(e => e.id);
                entity.Property(e => e.id).ValueGeneratedOnAdd();
                entity.HasIndex(e => e.email).IsUnique();
                entity.Property(e => e.created_at).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.updated_at).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            });

            // ... (otras configuraciones existentes)

            // ✅ NUEVAS CONFIGURACIONES para las tablas de seguimiento
            modelBuilder.Entity<EstadoEnvio>(entity =>
            {
                entity.ToTable("estado_envio");
                entity.HasKey(e => e.id);

                entity.Property(e => e.id).ValueGeneratedOnAdd();
                entity.Property(e => e.nombre).HasMaxLength(100).IsRequired();
                entity.Property(e => e.descripcion).HasColumnType("text");
                entity.Property(e => e.creado_en).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.actualizado_en).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<SeguimientoEnvio>(entity =>
            {
                entity.ToTable("seguimiento_envio");
                entity.HasKey(e => e.id);

                entity.Property(e => e.id).ValueGeneratedOnAdd();
                entity.Property(e => e.venta_id).IsRequired();
                entity.Property(e => e.estado_id).IsRequired();
                entity.Property(e => e.ubicacion_actual).HasMaxLength(255);
                entity.Property(e => e.observaciones).HasColumnType("text");
                entity.Property(e => e.fecha_cambio).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.confirmado_por_cliente).HasDefaultValue(false);
                entity.Property(e => e.creado_en).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.actualizado_en).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Relaciones
                entity.HasOne(se => se.Venta)
                    .WithMany()
                    .HasForeignKey(se => se.venta_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_seguimiento_venta");

                entity.HasOne(se => se.EstadoEnvio)
                    .WithMany()
                    .HasForeignKey(se => se.estado_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_seguimiento_estado");
            });

            modelBuilder.Entity<DocumentoEnvio>(entity =>
            {
                entity.ToTable("documento_envio");
                entity.HasKey(e => e.id);

                entity.Property(e => e.id).ValueGeneratedOnAdd();
                entity.Property(e => e.venta_id).IsRequired();
                entity.Property(e => e.tipo_documento).HasMaxLength(50).IsRequired();
                entity.Property(e => e.nombre_archivo).HasMaxLength(255).IsRequired();
                entity.Property(e => e.ruta_archivo).HasMaxLength(500).IsRequired();
                entity.Property(e => e.fecha_subida).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.creado_en).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.actualizado_en).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

                // Relación
                entity.HasOne(de => de.Venta)
                    .WithMany()
                    .HasForeignKey(de => de.venta_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_documento_venta");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}

