using FluentMigrator;

namespace Infrastructure.NHibernate.Migrations.Migrations
{
    [Migration(20250603195650)]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Create.Table("REVINFO")
              .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
              .WithColumn("RevisionDate").AsDateTime().Nullable();
        }

        public override void Down()
        {
            Delete.Table("REVINFO");
        }
    }
}