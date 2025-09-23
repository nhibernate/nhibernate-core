using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3607
{
	public class LineItemMapping : ClassMapping<LineItem>
	{
		public LineItemMapping()
		{
			Id(x => x.Id, m => m.Generator(new IdentityGeneratorDef()));

			Property(x => x.ItemName);

			Property(x => x.Amount);

			ManyToOne(x => x.ParentOrder);

			ManyToOne(x => x.Data);
		}
	}
}
