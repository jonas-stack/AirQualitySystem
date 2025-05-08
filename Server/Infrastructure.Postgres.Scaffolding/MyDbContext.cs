using System;
using System.Collections.Generic;
using Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SensorData> SensorData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("repmgr", "repmgr");

        modelBuilder.Entity<SensorData>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SensorData_pkey");

            entity.HasIndex(e => new { e.DeviceId, e.Timestamp }, "uq_deviceid_timestamp").IsUnique();

            entity.Property(e => e.Pm25).HasColumnName("PM25");
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
