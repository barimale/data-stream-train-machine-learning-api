namespace Card.Infrastructure.EntityConfigurations;

class CardEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.CardAggregate.Model>
{
    public void Configure(EntityTypeBuilder<Domain.AggregatesModel.CardAggregate.Model> cardConfiguration)
    {
        cardConfiguration.ToTable("cards");

        cardConfiguration.Property(o => o.Id);
        
        cardConfiguration
        .HasIndex(card => new
        {
            card.Id
        })
        .IsUnique();
    }
}
