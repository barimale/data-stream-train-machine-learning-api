using Demo.Domain.AggregatesModel.CompanyAggregate;
using Demo.Migrations.Conventions;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class AddressMap: ClassMap<Address>
    {
        public AddressMap()
        {
            //Table("Address")/*;*/
            Id(x => x.Id).GeneratedBy.TriggerIdentity();
            Map(x => x.Street).Length(100).Nullable();
            Map(x => x.City).Length(50).Nullable();
            Map(x => x.State).Length(50).Nullable();
            Map(x => x.ZipCode).Length(20).Nullable();
            Map(x => x.Country).Length(50).Nullable();
            Map(x => x.Phone).Length(50).Nullable();
            Version(u => u.Version);

            // Define the relationship with CompanyAddress
            HasMany(x => x.Companies)
                .Table(LowercaseTableNameConvention.TablePrefix + "CompanyAddress")
                .KeyColumn("AddressId")
                .Inverse()
                .Cascade.All();
        }
    }
}
