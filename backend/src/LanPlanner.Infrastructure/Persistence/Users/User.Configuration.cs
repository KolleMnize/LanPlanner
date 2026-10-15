using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanPlanner.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanPlanner.Infrastructure.Persistence.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public virtual void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasConversion(id => id.Value, value => UserId.Create(value).Value)
            .ValueGeneratedNever();

        builder.Property(u => u.Email).HasColumnType("citext").IsRequired();
        builder.Property(u => u.Name).HasColumnType("citext").IsRequired();

        builder.HasIndex(u => u.Email).IsUnique().HasDatabaseName("EmailIndex");
        builder.HasIndex(u => u.Name).IsUnique().HasDatabaseName("NameIndex");
    }
}
