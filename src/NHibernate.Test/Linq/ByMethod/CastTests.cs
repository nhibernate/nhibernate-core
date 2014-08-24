using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Linq.ByMethod
{
	public class CastTests : LinqTestCase
	{
		[Test]
		public void CastCount()
		{
			session.Query<Cat>()
				.Cast<Animal>()
				.Count().Should().Be(1);
		}

		[Test]
		public void CastWithWhere()
		{
			var pregnatMammal = (from a
			                      	in session.Query<Animal>().Cast<Cat>()
			                      where a.Pregnant
			                      select a).FirstOrDefault();
			pregnatMammal.Should().Not.Be.Null();
		}

		[Test]
		public void CastDowncast()
		{
			var query = session.Query<Mammal>().Cast<Dog>();
			// the list contains at least one Cat then should Throws
			query.Executing(q=> q.ToList()).Throws();
		}

		[Test]
		public void OrderByAfterCast()
		{
			// NH-2657
			var query = session.Query<Dog>().Cast<Animal>().OrderBy(a=> a.BodyWeight);
			query.Executing(q => q.ToList()).NotThrows();
		}

		[Test, Ignore("Not fixed yet. The method OfType does not work as expected.")]
		public void CastDowncastUsingOfType()
		{
			var query = session.Query<Animal>().OfType<Mammal>().Cast<Dog>();
			// the list contains at least one Cat then should Throws
			query.Executing(q => q.ToList()).Throws();
		}
	}
}