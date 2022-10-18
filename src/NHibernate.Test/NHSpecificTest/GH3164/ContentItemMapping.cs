using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3164
{
	public class ContentItemMapping : ClassMapping<ContentItem>
	{
		public ContentItemMapping()
		{
			Table("ContentItems");
			Id(x => x.Id, m => m.Generator(Generators.Identity));
			Property(x => x.Name);
			Component(x => x.Head, x => x.Lazy(true));
		}
	}
}
