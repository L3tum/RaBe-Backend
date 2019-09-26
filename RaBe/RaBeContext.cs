using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RaBe.Model;

namespace RaBe
{
    public partial class RaBeContext : DbContext
    {
        public RaBeContext()
        {
        }

        public RaBeContext(DbContextOptions<RaBeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Arbeitsplatz> Arbeitsplatz { get; set; }
        public virtual DbSet<Fehler> Fehler { get; set; }
        public virtual DbSet<Kategorie> Kategorie { get; set; }
        public virtual DbSet<Lehrer> Lehrer { get; set; }
        public virtual DbSet<LehrerRaum> LehrerRaum { get; set; }
        public virtual DbSet<Raum> Raum { get; set; }
        public virtual DbSet<StandardFehler> StandardFehler { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlite("DataSource=./RaBe.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Arbeitsplatz>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasColumnType("int");

                entity.Property(e => e.RaumId)
                    .HasColumnName("raum_id")
                    .HasColumnType("int");

                entity.HasOne(d => d.Raum)
                    .WithMany(p => p.Arbeitsplatz)
                    .HasForeignKey(d => d.RaumId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Fehler>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.ArbeitsplatzId)
                    .HasColumnName("arbeitsplatz_id")
                    .HasColumnType("int");

                entity.Property(e => e.Beschreibung)
                    .HasColumnName("beschreibung")
                    .HasColumnType("varchar(5000)");

                entity.Property(e => e.KategorieId)
                    .HasColumnName("kategorie_id")
                    .HasColumnType("int");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Titel)
                    .IsRequired()
                    .HasColumnName("titel")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Arbeitsplatz)
                    .WithMany(p => p.Fehler)
                    .HasForeignKey(d => d.ArbeitsplatzId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Kategorie)
                    .WithMany(p => p.Fehler)
                    .HasForeignKey(d => d.KategorieId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Kategorie>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<Lehrer>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("INT")
                    .ValueGeneratedNever();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PasswordGeaendert)
                    .HasColumnName("passwordGeaendert")
                    .HasColumnType("int")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasColumnType("varchar(255)");
            });

            modelBuilder.Entity<LehrerRaum>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.Betreuer)
                    .HasColumnName("betreuer")
                    .HasColumnType("tinyint")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.LehrerId)
                    .HasColumnName("lehrer_id")
                    .HasColumnType("int");

                entity.Property(e => e.RaumId)
                    .HasColumnName("raum_id")
                    .HasColumnType("int");

                entity.HasOne(d => d.Lehrer)
                    .WithMany(p => p.LehrerRaum)
                    .HasForeignKey(d => d.LehrerId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Raum)
                    .WithMany(p => p.LehrerRaum)
                    .HasForeignKey(d => d.RaumId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Raum>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Vorlage)
                    .HasColumnName("vorlage")
                    .HasColumnType("int");
            });

            modelBuilder.Entity<StandardFehler>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int")
                    .ValueGeneratedNever();

                entity.Property(e => e.Beschreibung)
                    .HasColumnName("beschreibung")
                    .HasColumnType("varchar(5000)");

                entity.Property(e => e.KategorieId)
                    .HasColumnName("kategorie_id")
                    .HasColumnType("int");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .HasColumnType("int")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Titel)
                    .IsRequired()
                    .HasColumnName("titel")
                    .HasColumnType("varchar(255)");

                entity.HasOne(d => d.Kategorie)
                    .WithMany(p => p.StandardFehler)
                    .HasForeignKey(d => d.KategorieId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
