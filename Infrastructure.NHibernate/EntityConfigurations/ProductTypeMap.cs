using Demo.Domain.AggregatesModel.ProductAggregate;
using FluentNHibernate.Mapping;

namespace Demo.Infrastructure.EntityConfigurations
{
    public class ProductTypeMap : ClassMap<ProductType>
    {
        public ProductTypeMap()
        {
            //Table("ProductType");
            Id(u => u.Id).GeneratedBy.Increment().Not.Nullable();
            Map(u => u.Description).Length(50).Nullable();
        }
    }
}
