using System.Linq;
using NHibernate.DomainModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH2463
{
	[TestFixture]
	public class Fixture : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] {"ABC.hbm.xml"}; }
		}

		[Test]
		public void CanJoinOnEntityWithDiscriminator()
		{
			using (var s = OpenSession())
			{
				s.Query<A>().Join(
					s.Query<A>(),
					a => a.Id,
					b => b.Id,
					(a, b) =>
						new {a, b}).ToList();
			}
		}
	}
}
