using Demo.Migrations.Conventions;
using FluentMigrator;

namespace Demo.Migrations.Migrations
{
    [Migration(20250606194233)]
    public class AddCompany2 : Migration
    {
        private readonly string TableName = LowercaseTableNameConvention.TablePrefix + "Company2";

        public override void Up()
        {
            // Main table
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Version").AsInt32()
                .WithColumn("Foo").AsString().Nullable();

            // Audit table
            Create.Table(TableName + "_AUD")
                .WithColumn("Id").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32().NotNullable()
                .WithColumn("REVTYPE").AsInt32().NotNullable()
                .WithColumn("Version").AsInt32().Nullable()
                .WithColumn("Foo").AsString().Nullable();

            Create.PrimaryKey("PK_" + TableName + "_AUD")
                .OnTable(TableName + "_AUD")
                .Columns("Id", "REV");

            Create.ForeignKey("FK_" + TableName + "_AUD_REVINFO")
                .FromTable(TableName + "_AUD").ForeignColumn("REV")
                .ToTable("REVINFO").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.Table(TableName + "_AUD");
            Delete.Table(TableName);
        }
    }
}