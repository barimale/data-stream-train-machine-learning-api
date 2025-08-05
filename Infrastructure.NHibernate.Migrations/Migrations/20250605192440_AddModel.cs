using FluentMigrator;
using Infrastructure.NHibernate.Migrations.Conventions;

namespace Infrastructure.NHibernate.Migrations.Migrations
{
    [Migration(20250605192440)]
    public class AddAddress : Migration
    {
        private readonly string TableName = LowercaseTableNameConvention.TablePrefix + "Model";

        public override void Up()
        {
            Create.Table(TableName)
               .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
               .WithColumn("RegisteringTime").AsDateTimeOffset()
               .WithColumn("ModelAsBytes").AsBinary().Nullable() // wip asbyte
               .WithColumn("Version").AsInt32()
               .WithColumn("ModelVersion").AsString();

            // Audit table
            Create.Table(TableName + "_AUD")
                .WithColumn("Id").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32().NotNullable()
                .WithColumn("REVTYPE").AsInt32().NotNullable()
                .WithColumn("RegisteringTime").AsDateTimeOffset().Nullable()
                .WithColumn("ModelAsBytes").AsBinary().Nullable()
                .WithColumn("Version").AsInt32().Nullable()
                .WithColumn("ModelVersion").AsString();

            Create.PrimaryKey("PK_" + TableName + "_AUD")
                .OnTable(TableName + "_AUD")
                .Columns("Id", "REV");

            Create.ForeignKey("FK_" + TableName + "_AUD_REVINFO")
                .FromTable(TableName + "_AUD").ForeignColumn("REV")
                .ToTable("REVINFO").PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}