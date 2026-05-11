using LogServer.Core.Logs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LogServer.Infrastructure.Persistence.Configurations;

public sealed class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.ToTable("Logs");

        builder.HasKey(x => x.LogId);

        builder.Property(x => x.LogLevel)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(x => x.ClientId)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.Ignore(x => x.DomainEvents);
    }
}
