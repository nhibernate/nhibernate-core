using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Example.Web.Models
{
	public class ItemMap : ClassMapping<Item>
	{
		public ItemMap()
		{
			Lazy(false);
			Id(x => x.Id, map => map.Generator(Generators.Native));
			Property(x => x.Price);
			Property(x => x.Description);
		}
	}
}
