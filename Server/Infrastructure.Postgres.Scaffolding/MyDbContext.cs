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

    public virtual DbSet<DeviceConnectionHistory> DeviceConnectionHistory { get; set; }

    public virtual DbSet<Devices> Devices { get; set; }

    public virtual DbSet<SensorData> SensorData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeviceConnectionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("DeviceConnectionHistory_pkey");

            entity.Property(e => e.LastSeen).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<Devices>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("Devices_pkey");

            entity.Property(e => e.DeviceId).ValueGeneratedNever();
            entity.Property(e => e.DeviceName).HasMaxLength(100);
            entity.Property(e => e.IsConnected).HasDefaultValue(false);
            entity.Property(e => e.LastSeen).HasColumnType("timestamp without time zone");
            entity.Property(e => e.Updateinterval).HasDefaultValue(30000);
        });

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
