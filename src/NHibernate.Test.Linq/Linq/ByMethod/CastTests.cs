using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	public class CastTests : LinqTestCase
	{
		[Test]
		public void CastCount()
		{
			Assert.That(session.Query<Cat>()
							   .Cast<Animal>()
							   .Count(), Is.EqualTo(1));
		}

		[Test]
		public void CastWithWhere()
		{
			var pregnatMammal = (from a
									in session.Query<Animal>().Cast<Cat>()
								  where a.Pregnant
								  select a).FirstOrDefault();
			Assert.That(pregnatMammal, Is.Not.Null);
		}

		[Test]
		public void CastDowncast()
		{
			var query = session.Query<Mammal>().Cast<Dog>();
			// the list contains at least one Cat then should Throws
			Assert.That(() => query.ToList(), Throws.Exception);
		}

		[Test]
		public void OrderByAfterCast()
		{
			// NH-2657
			var query = session.Query<Dog>().Cast<Animal>().OrderBy(a=> a.BodyWeight);
			Assert.That(() => query.ToList(), Throws.Nothing);
		}

		[Test, Ignore("Not fixed yet. The method OfType does not work as expected.")]
		public void CastDowncastUsingOfType()
		{
			var query = session.Query<Animal>().OfType<Mammal>().Cast<Dog>();
			// the list contains at least one Cat then should Throws
			Assert.That(() => query.ToList(), Throws.Exception);
		}
	}
}