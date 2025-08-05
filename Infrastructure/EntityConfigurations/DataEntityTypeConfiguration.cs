using Domain.AggregatesModel.DataAggregate;

namespace Infrastructure.EFCore.EntityConfigurations;

class DataEntityTypeConfiguration : IEntityTypeConfiguration<Data>
{
    public void Configure(EntityTypeBuilder<Data> cardConfiguration)
    {
        cardConfiguration.ToTable("data");

        cardConfiguration.Property(o => o.Id);
        
        cardConfiguration
        .HasIndex(card => new
        {
            card.Id
        })
        .IsUnique();
    }
}
