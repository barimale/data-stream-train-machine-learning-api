using Demo.Migrations.Conventions;
using FluentMigrator;

namespace Demo.Migrations.Migrations
{
    [Migration(20250606194224)]
    public class AddAddress2 : Migration
    {
        private readonly string TableName = LowercaseTableNameConvention.TablePrefix + "Address2";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("Street").AsString().Nullable()
                .WithColumn("City").AsString().Nullable()
                .WithColumn("State").AsString().Nullable()
                .WithColumn("ZipCode").AsString().Nullable()
                .WithColumn("Country").AsString().Nullable()
                .WithColumn("Version").AsInt32()
                .WithColumn("Phone").AsString().Nullable();

            // Audit table
            Create.Table(TableName + "_AUD")
                .WithColumn("Id").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32().NotNullable()
                .WithColumn("REVTYPE").AsInt32().NotNullable()
                .WithColumn("Street").AsString().Nullable()
                .WithColumn("City").AsString().Nullable()
                .WithColumn("State").AsString().Nullable()
                .WithColumn("ZipCode").AsString().Nullable()
                .WithColumn("Country").AsString().Nullable()
                .WithColumn("Version").AsInt32().Nullable()
                .WithColumn("Phone").AsString().Nullable();

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