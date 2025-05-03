namespace Card.Infrastructure.EntityConfigurations;

class CardEntityTypeConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.CardAggregate.Card>
{
    public void Configure(EntityTypeBuilder<Domain.AggregatesModel.CardAggregate.Card> cardConfiguration)
    {
        cardConfiguration.ToTable("cards");

        cardConfiguration.Property(o => o.Id);
        cardConfiguration
            .HasIndex(card => new
            {
                card.SerialNumber
            })
            .IsUnique();
        cardConfiguration
        .HasIndex(card => new
        {
            card.AccountNumber
        })
        .IsUnique();
        cardConfiguration
        .HasIndex(card => new
        {
            card.Id
        })
        .IsUnique();
    }
}
