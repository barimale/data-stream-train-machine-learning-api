using Microsoft.EntityFrameworkCore.Design;

namespace Card.Infrastructure
{
    /* For migrations generation only */

    public class NNContextFactory : IDesignTimeDbContextFactory<NNContext>
    {
        public NNContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NNContext>();
            optionsBuilder.UseSqlServer("Data Source=localhost,1500;Database=DNNdb;Uid=sa;Password=Password_123#;TrustServerCertificate=True");

            return new NNContext(optionsBuilder.Options);
        }
    }
}