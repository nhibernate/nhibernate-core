using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.GH3607
{
	public class OrderMapping : ClassMapping<Order>
	{
		public OrderMapping()
		{
			Table("`Order`");
			Id(x => x.Id, m => m.Generator(new IdentityGeneratorDef()));

			Property(x => x.CreatedDate);

			Set(x => x.Items, m =>
			{
				m.Inverse(true);
				m.OptimisticLock(true);
			}, a => a.OneToMany());
		}
	}
}
