using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3164
{
	public class HeadMapping : ComponentMapping<IHead>
	{
		public HeadMapping()
		{
			Class<Head>();
			Property(x => x.Title, m =>
			{
				m.Column("PageTitle");
				m.Lazy(true);
			});
			Property(x => x.DummyFieldToLoadEmptyComponent, m =>
			{
				m.Access(Accessor.ReadOnly);
				m.Formula("1");
			});
			Lazy(true);
		}
	}
}
