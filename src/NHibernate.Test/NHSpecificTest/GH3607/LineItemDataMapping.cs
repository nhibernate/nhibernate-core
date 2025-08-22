using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3607
{
	public class LineItemDataMapping : ClassMapping<LineItemData>
	{
		public LineItemDataMapping()
		{
			OneToOne(x => x.LineItem, m => m.Constrained(true));

			Property(x => x.Data);
		}
	}
}
