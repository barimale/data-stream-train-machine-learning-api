using Demo.Domain.AggregatesModel.Company2Aggregate;
using Demo.Migrations.Conventions;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class Company2Map : ClassMap<Company2>
    {
        public Company2Map()
        {
            //Table("Company")/*;*/
            Id(x => x.Id).GeneratedBy.TriggerIdentity();
            Map(x => x.Foo).Length(50).Nullable();
            Version(u => u.Version);

            HasManyToMany(x => x.Addresses)
                .Table(LowercaseTableNameConvention.TablePrefix + "CompanyAddress2")
                .ParentKeyColumn("CompanyId")
                .ChildKeyColumn("AddressId")
                .Cascade.All().Fetch.Select();
        }
    }
}
