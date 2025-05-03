using Card.Domain.AggregatesModel.CardAggregate;

namespace Card.Infrastructure.EntityConfigurations;

class DataEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.CardAggregate.Data>
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
