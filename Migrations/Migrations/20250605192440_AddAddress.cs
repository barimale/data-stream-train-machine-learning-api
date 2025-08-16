using Demo.Migrations.Conventions;
using FluentMigrator;

namespace Demo.Migrations.Migrations
{
    [Migration(20250605192440)]
    public class AddAddress : Migration
    {
        private readonly string TableName = LowercaseTableNameConvention.TablePrefix + "Address";

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
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}