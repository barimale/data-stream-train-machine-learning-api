using Demo.Migrations.Conventions;
using FluentMigrator;

namespace Demo.Migrations.Migrations
{
    [Migration(20250603195651)]
    public class AddDataMigration : Migration
    {
        private readonly string TableName = LowercaseTableNameConvention.TablePrefix + "Data";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().PrimaryKey().Identity()
                .WithColumn("IngestionTime").AsDateTimeOffset()
                .WithColumn("Xs").AsString()
                .WithColumn("Ys").AsString()
                .WithColumn("PieceOfModel").AsBinary().Nullable() // wip asbyte
                .WithColumn("Version").AsInt32()
                .WithColumn("IsApplied").AsBoolean().WithDefaultValue(false);

            // Audit table
            Create.Table(TableName + "_AUD")
                .WithColumn("Id").AsInt32().NotNullable()
                .WithColumn("REV").AsInt32().NotNullable()
                .WithColumn("REVTYPE").AsInt32().NotNullable()
                .WithColumn("IngestionTime").AsDateTimeOffset().Nullable()
                .WithColumn("Xs").AsString().Nullable()
                .WithColumn("Ys").AsString().Nullable()
                .WithColumn("PieceOfModel").AsBinary().Nullable()
                .WithColumn("Version").AsInt32().Nullable()
                .WithColumn("IsApplied").AsBoolean().Nullable().WithDefaultValue(false);

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