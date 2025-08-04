using Demo.Migrations.Conventions;
using FluentMigrator;

namespace Demo.Migrations.Migrations
{
    [Migration(20250605193138)]
    public class AddCompany : Migration
    {
        private readonly string TableName = LowercaseTableNameConvention.TablePrefix + "Company";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Version").AsInt32()
                .WithColumn("Foo").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}