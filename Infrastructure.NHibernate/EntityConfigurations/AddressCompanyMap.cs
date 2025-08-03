using Demo.Domain.AggregatesModel.CompanyAggregate;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class AddressCompanyMap : ClassMap<CompanyAddress>
    {
        public AddressCompanyMap()
        {
            //Table("AddressCompany")/*;*/
            Id(x => x.Id).GeneratedBy.TriggerIdentity();
            Id(x => x.Address.Id).GeneratedBy.TriggerIdentity().Not.Nullable();
            Id(x => x.Company.Id).GeneratedBy.TriggerIdentity().Not.Nullable();
            Map(x => x.Description).Length(200).Nullable();
            Map(x => x.CreationDate).Not.Nullable();
            
            References(x => x.Address)
                .Column("AddressId")
                .Not.Nullable()
                .Cascade.None(); // Adjust cascade as needed
            References(x => x.Company)
                .Column("CompanyId")
                .Not.Nullable()
                .Cascade.None(); // Adjust cascade as needed
        }
    }
}
