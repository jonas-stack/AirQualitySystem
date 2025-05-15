using System;
using System.Collections.Generic;
using Core.Domain.TestEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding;

public partial class MyDbContextDocker : DbContext
{
    public MyDbContextDocker(DbContextOptions<MyDbContextDocker> options)
        : base(options)
    {
    }

    public virtual DbSet<TestDeviceConnectionHistory> TestDeviceConnectionHistory { get; set; }

    public virtual DbSet<TestDevices> TestDevices { get; set; }

    public virtual DbSet<TestInvalidPayloads> TestInvalidPayloads { get; set; }

    public virtual DbSet<TestSensorData> TestSensorData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestDeviceConnectionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TestDeviceConnectionHistory_pkey");

            entity.Property(e => e.LastSeen).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Device).WithMany(p => p.TestDeviceConnectionHistory)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceConnection_Devices");
        });

        modelBuilder.Entity<TestDevices>(entity =>
        {
            entity.HasKey(e => e.DeviceId).HasName("TestDevices_pkey");

            entity.Property(e => e.DeviceId).ValueGeneratedNever();
            entity.Property(e => e.DeviceName).HasMaxLength(100);
            entity.Property(e => e.IsConnected).HasDefaultValue(false);
            entity.Property(e => e.LastSeen).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<TestInvalidPayloads>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TestInvalidPayloads_pkey");

            entity.Property(e => e.DeviceId).HasMaxLength(100);
            entity.Property(e => e.ErrorReason).HasMaxLength(255);
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<TestSensorData>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("TestSensorData_pkey");

            entity.HasIndex(e => new { e.DeviceId, e.Timestamp }, "uq_deviceid_timestamp").IsUnique();

            entity.Property(e => e.Pm25).HasColumnName("PM25");
            entity.Property(e => e.Timestamp).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.Device).WithMany(p => p.TestSensorData)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TestSensorData_Devices");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
