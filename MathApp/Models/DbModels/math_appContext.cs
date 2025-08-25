using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace MathApp.Models.DbModels
{
    public partial class math_appContext : DbContext
    {
        public math_appContext()
        {
            ChangeTracker.LazyLoadingEnabled = true;
        }

        public math_appContext(DbContextOptions<math_appContext> options)
            : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = true;

        }

        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<ChangePasswordCode> ChangePasswordCodes { get; set; }
        public virtual DbSet<MathProblem> MathProblems { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;user=root;database=math_app;port=3307;password=%s1WnX6*", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.26-mysql"));
            }
        }*/

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_0900_ai_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.ToTable("answers");

                entity.HasIndex(e => e.MathProblem, "fkzadacha_idx");

                entity.HasIndex(e => e.Id, "id_answer_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.Property(e => e.Validity).HasColumnName("validity");

                entity.Property(e => e.MathProblem).HasColumnName("math_problem");

                entity.HasOne(d => d.MathProblemNavigation)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.MathProblem)
                    .HasConstraintName("fkzadacha");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.HasIndex(e => e.Id, "id_category_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Difficulty)
                    .HasMaxLength(1)
                    .HasColumnName("difficulty");

                entity.Property(e => e.Grade).HasColumnName("grade");
            });


            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles");

                entity.HasIndex(e => e.Id, "id_role_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Topic>(entity =>
            {
                entity.ToTable("topics");

                entity.HasIndex(e => e.IdUser, "fkuser1_idx");

                entity.HasIndex(e => e.Id, "id_topic_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date");

                entity.Property(e => e.Deletionstatus).HasColumnName("deletionstatus");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.EventDate).HasColumnName("event_date");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Type)
                    .HasMaxLength(10)
                    .HasColumnName("type");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Topics)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkuser1");

                entity.HasMany(d => d.IdCategories)
                    .WithMany(p => p.IdTopics)
                    .UsingEntity<Dictionary<string, object>>(
                        "JunctionTopicCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("IdCategory").HasConstraintName("ftcat"),
                        r => r.HasOne<Topic>().WithMany().HasForeignKey("IdTopic").HasConstraintName("ftem"),
                        j =>
                        {
                            j.HasKey("IdTopic", "IdCategory").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("junction_topics_categories");

                            j.HasIndex(new[] { "IdCategory" }, "ftcat_idx");

                            j.IndexerProperty<int>("IdTopic").HasColumnName("id_topic");

                            j.IndexerProperty<int>("IdCategory").HasColumnName("id_category");
                        });
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.HasIndex(e => e.Email, "email_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.EmailConfirmation).HasColumnName("email_confirmation");
                entity.Property(e => e.IsDisabled).HasColumnName("is_disabled");

                

                entity.HasIndex(e => e.IdRole, "fkrole_idx");

                entity.HasIndex(e => e.Id, "id_user_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(60)
                    .HasColumnName("email");

                

                entity.Property(e => e.IdRole).HasColumnName("id_role");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(84)
                    .HasColumnName("password")
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("name");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("username");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date");


                entity.HasOne(d => d.IdRoleNavigation)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.IdRole)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fkrole");
            });
            modelBuilder.Entity<ChangePasswordCode>(entity =>
            {
                entity.ToTable("change_password_codes");

                entity.HasIndex(e => e.Id, "id_change_password_codes_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(8)
                    .HasColumnName("code");

                entity.HasIndex(e => e.IdUser, "fk_usercode_idx");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.PasswordCodes)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("fk_usercode");

                entity.Property(e => e.ExpiresAt)
                    .HasColumnType("datetime")
                    .HasColumnName("expires_at");

                entity.Property(e => e.IsValid).HasColumnName("is_valid");

            });

            modelBuilder.Entity<MathProblem>(entity =>
            {
                entity.ToTable("mathProblems");

                entity.HasIndex(e => e.IdTopic, "fktema_idx");

                entity.HasIndex(e => e.IdUser, "fkuser2_idx");

                entity.HasIndex(e => e.Id, "id_math_problem_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("creation_date");

                entity.Property(e => e.Deletionstatus).HasColumnName("deletionstatus");

                entity.Property(e => e.IdTopic).HasColumnName("id_topic");

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.Position).HasColumnName("position");

                entity.Property(e => e.Solution)
                    .HasMaxLength(3000)
                    .HasColumnName("solution")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.Conditions)
                    .IsRequired()
                    .HasMaxLength(3000)
                    .HasColumnName("conditions")
                    .UseCollation("utf8_general_ci")
                    .HasCharSet("utf8");

                entity.HasOne(d => d.IdTopicNavigation)
                    .WithMany(p => p.MathProblems)
                    .HasForeignKey(d => d.IdTopic)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fktema");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.MathProblems)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fkuser2");

                entity.HasMany(d => d.Categories)
                    .WithMany(p => p.IdMathProblems)
                    .UsingEntity<Dictionary<string, object>>(
                        "JunctionMathProblemsCategory",
                        l => l.HasOne<Category>().WithMany().HasForeignKey("IdCategory").HasConstraintName("fkcat"),
                        r => r.HasOne<MathProblem>().WithMany().HasForeignKey("IdMathProblem").HasConstraintName("kdzad2"),
                        j =>
                        {
                            j.HasKey("IdMathProblem", "IdCategory").HasName("PRIMARY").HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

                            j.ToTable("junction_math_problems_categories");

                            j.HasIndex(new[] { "IdCategory" }, "fkcat_idx");

                            j.IndexerProperty<int>("IdMathProblem").HasColumnName("id_math_problem");

                            j.IndexerProperty<int>("IdCategory").HasColumnName("id_category");
                        });
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
