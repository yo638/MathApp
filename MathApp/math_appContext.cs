using System;
using MathApp.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MathApp
{
    public partial class math_appContext : DbContext
    {
        public math_appContext()
        {
        }

        public math_appContext(DbContextOptions<math_appContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answers> Answers { get; set; }
        public virtual DbSet<Categories> Categories { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<JunctionTemiCategories> JunctionTemiCategories { get; set; }
        public virtual DbSet<JunctionZadachiAnswers> JunctionZadachiAnswers { get; set; }
        public virtual DbSet<JunctionZadachiCategories> JunctionZadachiCategories { get; set; }
        public virtual DbSet<JunctionZadachiImages> JunctionZadachiImages { get; set; }
        public virtual DbSet<JunctionZadachiTemi> JunctionZadachiTemi { get; set; }
        public virtual DbSet<Temi> Temi { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Zadachi> Zadachi { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=localhost;user=root;database=math_app;port=3307;password=%s1WnX6*");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answers>(entity =>
            {
                entity.HasKey(e => e.IdAnswer)
                    .HasName("PRIMARY");

                entity.ToTable("answers");

                entity.HasIndex(e => e.IdAnswer)
                    .HasName("id_answer_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdAnswer).HasColumnName("id_answer");

                entity.Property(e => e.Answer)
                    .IsRequired()
                    .HasColumnName("answer")
                    .HasMaxLength(200);

                entity.Property(e => e.Validity)
                    .HasColumnName("validity")
                    .HasColumnType("tinyint");
            });

            modelBuilder.Entity<Categories>(entity =>
            {
                entity.HasKey(e => e.IdCategory)
                    .HasName("PRIMARY");

                entity.ToTable("categories");

                entity.HasIndex(e => e.IdCategory)
                    .HasName("id_category_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdCategory).HasColumnName("id_category");

                entity.Property(e => e.Difficulty)
                    .HasColumnName("difficulty")
                    .HasMaxLength(6);

                entity.Property(e => e.Grade).HasColumnName("grade");
            });

            modelBuilder.Entity<Images>(entity =>
            {
                entity.HasKey(e => e.IdImage)
                    .HasName("PRIMARY");

                entity.ToTable("images");

                entity.HasIndex(e => e.IdImage)
                    .HasName("id_image_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.Image)
                    .HasName("image_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdImage).HasColumnName("id_image");

                entity.Property(e => e.Image)
                    .IsRequired()
                    .HasColumnName("image")
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<JunctionTemiCategories>(entity =>
            {
                entity.HasKey(e => new { e.Tema, e.Category })
                    .HasName("PRIMARY");

                entity.ToTable("junction_temi_categories");

                entity.HasIndex(e => e.Category)
                    .HasName("ftcat_idx");

                entity.Property(e => e.Tema).HasColumnName("tema");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.JunctionTemiCategories)
                    .HasForeignKey(d => d.Category)
                    .HasConstraintName("ftcat");

                entity.HasOne(d => d.TemaNavigation)
                    .WithMany(p => p.JunctionTemiCategories)
                    .HasForeignKey(d => d.Tema)
                    .HasConstraintName("ftem");
            });

            modelBuilder.Entity<JunctionZadachiAnswers>(entity =>
            {
                entity.HasKey(e => new { e.Zadacha, e.Answer })
                    .HasName("PRIMARY");

                entity.ToTable("junction_zadachi_answers");

                entity.HasIndex(e => e.Answer)
                    .HasName("fkans_idx");

                entity.Property(e => e.Zadacha).HasColumnName("zadacha");

                entity.Property(e => e.Answer).HasColumnName("answer");

                entity.HasOne(d => d.AnswerNavigation)
                    .WithMany(p => p.JunctionZadachiAnswers)
                    .HasForeignKey(d => d.Answer)
                    .HasConstraintName("fkans");

                entity.HasOne(d => d.ZadachaNavigation)
                    .WithMany(p => p.JunctionZadachiAnswers)
                    .HasForeignKey(d => d.Zadacha)
                    .HasConstraintName("fkzad");
            });

            modelBuilder.Entity<JunctionZadachiCategories>(entity =>
            {
                entity.HasKey(e => new { e.Zadacha, e.Category })
                    .HasName("PRIMARY");

                entity.ToTable("junction_zadachi_categories");

                entity.HasIndex(e => e.Category)
                    .HasName("fkcat_idx");

                entity.Property(e => e.Zadacha).HasColumnName("zadacha");

                entity.Property(e => e.Category).HasColumnName("category");

                entity.HasOne(d => d.CategoryNavigation)
                    .WithMany(p => p.JunctionZadachiCategories)
                    .HasForeignKey(d => d.Category)
                    .HasConstraintName("fkcat");

                entity.HasOne(d => d.ZadachaNavigation)
                    .WithMany(p => p.JunctionZadachiCategories)
                    .HasForeignKey(d => d.Zadacha)
                    .HasConstraintName("kdzad2");
            });

            modelBuilder.Entity<JunctionZadachiImages>(entity =>
            {
                entity.HasKey(e => new { e.Zadacha, e.Image, e.Place })
                    .HasName("PRIMARY");

                entity.ToTable("junction_zadachi_images");

                entity.HasIndex(e => e.Image)
                    .HasName("fkim_idx");

                entity.Property(e => e.Zadacha).HasColumnName("zadacha");

                entity.Property(e => e.Image).HasColumnName("image");

                entity.Property(e => e.Place).HasColumnName("place");

                entity.HasOne(d => d.ImageNavigation)
                    .WithMany(p => p.JunctionZadachiImages)
                    .HasForeignKey(d => d.Image)
                    .HasConstraintName("fkim");

                entity.HasOne(d => d.ZadachaNavigation)
                    .WithMany(p => p.JunctionZadachiImages)
                    .HasForeignKey(d => d.Zadacha)
                    .HasConstraintName("fkzad3");
            });

            modelBuilder.Entity<JunctionZadachiTemi>(entity =>
            {
                entity.HasKey(e => new { e.Tema, e.Zadacha })
                    .HasName("PRIMARY");

                entity.ToTable("junction_zadachi_temi");

                entity.HasIndex(e => e.Zadacha)
                    .HasName("fkzad4_idx");

                entity.Property(e => e.Tema).HasColumnName("tema");

                entity.Property(e => e.Zadacha).HasColumnName("zadacha");

                entity.Property(e => e.Number).HasColumnName("number");

                entity.HasOne(d => d.TemaNavigation)
                    .WithMany(p => p.JunctionZadachiTemi)
                    .HasForeignKey(d => d.Tema)
                    .HasConstraintName("fktem");

                entity.HasOne(d => d.ZadachaNavigation)
                    .WithMany(p => p.JunctionZadachiTemi)
                    .HasForeignKey(d => d.Zadacha)
                    .HasConstraintName("fkzad4");
            });

            modelBuilder.Entity<Temi>(entity =>
            {
                entity.HasKey(e => e.IdTema)
                    .HasName("PRIMARY");

                entity.ToTable("temi");

                entity.HasIndex(e => e.IdTema)
                    .HasName("id_tema_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.User)
                    .HasName("fkuser1_idx");

                entity.Property(e => e.IdTema).HasColumnName("id_tema");

                entity.Property(e => e.CreationDate).HasColumnName("creation_date");

                entity.Property(e => e.Deletionstatus)
                    .IsRequired()
                    .HasColumnName("deletionstatus")
                    .HasMaxLength(10);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(1000);

                entity.Property(e => e.EventDate)
                    .HasColumnName("event_date")
                    .HasColumnType("date");

                entity.Property(e => e.Tema)
                    .HasColumnName("tema")
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasMaxLength(10);

                entity.Property(e => e.UpdateDate).HasColumnName("update_date");

                entity.Property(e => e.User).HasColumnName("user");

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Temi)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkuser1");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.IdUser)
                    .HasName("PRIMARY");

                entity.ToTable("users");

                entity.HasIndex(e => e.Email)
                    .HasName("email_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.IdUser)
                    .HasName("id_user_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(60);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(84)
                    .IsFixedLength();

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(40);
            });

            modelBuilder.Entity<Zadachi>(entity =>
            {
                entity.HasKey(e => e.IdZadacha)
                    .HasName("PRIMARY");

                entity.ToTable("zadachi");

                entity.HasIndex(e => e.IdZadacha)
                    .HasName("id_zadacha_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.User)
                    .HasName("fkuser2_idx");

                entity.Property(e => e.IdZadacha).HasColumnName("id_zadacha");

                entity.Property(e => e.CreationDate).HasColumnName("creation_date");

                entity.Property(e => e.Solution)
                    .HasColumnName("solution")
                    .HasMaxLength(3000);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasMaxLength(10);

                entity.Property(e => e.UpdateDate).HasColumnName("update_date");

                entity.Property(e => e.User).HasColumnName("user");

                entity.Property(e => e.Uslovie)
                    .IsRequired()
                    .HasColumnName("uslovie")
                    .HasMaxLength(3000);

                entity.HasOne(d => d.UserNavigation)
                    .WithMany(p => p.Zadachi)
                    .HasForeignKey(d => d.User)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkuser2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
