using Demo.Migrations.Conventions;
using FluentMigrator;

namespace Demo.Migrations.Migrations
{
    [Migration(20250605193247)]
    public class AddCompanyAddress : Migration
    {
        private readonly string TableName = LowercaseTableNameConvention.TablePrefix + "CompanyAddress";
        private readonly string CompanyTable = LowercaseTableNameConvention.TablePrefix + "Company";
        private readonly string AddressTable = LowercaseTableNameConvention.TablePrefix + "Address";

        public override void Up()
        {
            Create.Table(TableName)
                .WithColumn("Id").AsInt32().NotNullable().Identity()
                .WithColumn("CompanyId").AsInt32().NotNullable()
                .WithColumn("AddressId").AsInt32().NotNullable()
                .WithColumn("Description").AsString().NotNullable()
                .WithColumn("CreationDate").AsDateTime2().NotNullable();

            Create.PrimaryKey("PK_CompanyAddress")
                .OnTable(TableName)
                .Columns("CompanyId", "AddressId");

            Create.ForeignKey("FK_CompanyAddress_Address")
                .FromTable(TableName).ForeignColumn("AddressId")
                .ToTable(AddressTable).PrimaryColumn("Id");

            Create.ForeignKey("FK_CompanyAddress_Company")
                .FromTable(TableName).ForeignColumn("CompanyId")
                .ToTable(CompanyTable).PrimaryColumn("Id");
        }

        public override void Down()
        {
            Delete.Table(TableName);
        }
    }
}