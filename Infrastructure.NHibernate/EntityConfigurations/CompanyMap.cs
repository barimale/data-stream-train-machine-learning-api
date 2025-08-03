using Demo.Domain.AggregatesModel.CompanyAggregate;
using Demo.Migrations.Conventions;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class CompanyMap : ClassMap<Company>
    {
        public CompanyMap()
        {
            //Table("Company")/*;*/
            Id(x => x.Id).GeneratedBy.TriggerIdentity();
            Map(x => x.Foo).Length(50).Nullable();
            Version(u => u.Version);

            HasMany(x => x.Addresses)
                .Table(LowercaseTableNameConvention.TablePrefix + "CompanyAddress")
                .KeyColumn("CompanyId")
                .Inverse()
                .Cascade.All();
        }
    }
}
