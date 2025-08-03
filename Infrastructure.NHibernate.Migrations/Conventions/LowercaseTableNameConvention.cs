using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Demo.Migrations.Conventions
{
    public class LowercaseTableNameConvention : IClassConvention
    {
        public static string TablePrefix = "tbl_"; // Prefix for table names
        public void Apply(IClassInstance instance)
        {
            instance.Table(TablePrefix + instance.EntityType.Name);
        }
    }
}
