using Microsoft.EntityFrameworkCore.Design;

namespace Card.Infrastructure
{
    /* For migrations generation only */

    public class CardContextFactory : IDesignTimeDbContextFactory<CardContext>
    {
        public CardContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CardContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost,1500;Database=CardManagerDB;Uid=sa;Password=Password_123#;TrustServerCertificate=True");

            return new CardContext(optionsBuilder.Options);
        }
    }
}