using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AuthSystem.API.Models;

public partial class AuthSystemNewDbContext : DbContext
{
    public AuthSystemNewDbContext()
    {
    }

    public AuthSystemNewDbContext(DbContextOptions<AuthSystemNewDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=AuthSystemNewDb;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasIndex(e => e.Name, "IX_EmailTemplates_Name").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.HtmlContent).IsRequired();
            entity.Property(e => e.LastModifiedBy)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.TextContent).IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
