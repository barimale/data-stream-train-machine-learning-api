using Domain.AggregatesModel.ModelAggregate;

namespace Infrastructure.EFCore.EntityConfigurations;

class CardEntityTypeConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> cardConfiguration)
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
