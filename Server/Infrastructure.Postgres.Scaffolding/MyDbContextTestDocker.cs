using System;
using System.Collections.Generic;
using Core.Domain.Entities;
using Core.Domain.TestEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Scaffolding;

public partial class MyDbContextTestDocker : DbContext
{
    public MyDbContextTestDocker(DbContextOptions<MyDbContextTestDocker> options)
        : base(options)
    {
    }

    public virtual DbSet<Devicestatus> Devicestatus { get; set; }

    public virtual DbSet<Sensordata> Sensordata { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Devicestatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("devicestatus_pkey");

            entity.ToTable("devicestatus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Deviceid)
                .HasMaxLength(50)
                .HasColumnName("deviceid");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<Sensordata>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sensordata_pkey");

            entity.ToTable("sensordata");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Airquality).HasColumnName("airquality");
            entity.Property(e => e.Deviceid)
                .HasMaxLength(50)
                .HasColumnName("deviceid");
            entity.Property(e => e.Humidity).HasColumnName("humidity");
            entity.Property(e => e.Pm25).HasColumnName("pm25");
            entity.Property(e => e.Temperature).HasColumnName("temperature");
            entity.Property(e => e.Timestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("timestamp");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
