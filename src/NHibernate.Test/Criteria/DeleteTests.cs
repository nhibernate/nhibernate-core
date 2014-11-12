using System.Collections;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Criteria
{
	[TestFixture]
	public class DeleteTests : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get
			{
				return new string[]
					{
						"Criteria.Enrolment.hbm.xml",
						"Criteria.Animal.hbm.xml",
						"Criteria.MaterialResource.hbm.xml"
					};
			}
		}

		[Test]
		public void CanDeleteSimpleExpression()
		{
			//NH-3735
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var mother = new Animal
				{
					BodyWeight = 200f,
					Description = "Mother animal",
					SerialNumber = "Mother"
				};

				var father = new Animal
				{
					BodyWeight = 250f,
					Description = "Father animal",
					SerialNumber = "Father"
				};

				var child = new Animal
				{
					BodyWeight = 100f,
					Description = "Child animal",
					SerialNumber = "Child",
					Mother = mother,
					Father = father
				};

				session.Save(mother);
				session.Save(father);
				session.Save(child);
				session.Flush();

				session.Clear();

				var beforeDeleteCount = session.CreateCriteria(typeof(Animal)).Add(Restrictions.IsNotNull("father")).SetProjection(Projections.RowCount()).UniqueResult<int>();

				var deleteCount = session.CreateCriteria(typeof(Animal)).Add(Restrictions.IsNotNull("father")).Delete();

				var afterDeleteCount = session.CreateCriteria(typeof(Animal)).Add(Restrictions.IsNotNull("father")).SetProjection(Projections.RowCount()).UniqueResult<int>();

				Assert.AreEqual(beforeDeleteCount, deleteCount);
				Assert.AreEqual(0, afterDeleteCount);
			}
		}

		[Test]
		public void CanDeleteComplexExpression()
		{
			//NH-3735
			using (var session = this.OpenSession())
			using (session.BeginTransaction())
			{
				var mother = new Animal
				{
					BodyWeight = 200f,
					Description = "Mother animal",
					SerialNumber = "Mother"
				};

				var father = new Animal
				{
					BodyWeight = 250f,
					Description = "Father animal",
					SerialNumber = "Father"
				};

				var child = new Animal
				{
					BodyWeight = 100f,
					Description = "Child animal",
					SerialNumber = "Child",
					Mother = mother,
					Father = father
				};

				session.Save(mother);
				session.Save(father);
				session.Save(child);
				session.Flush();

				session.Clear();

				var beforeDeleteCount = session.CreateCriteria(typeof(Animal)).Add(Restrictions.Conjunction().Add(Restrictions.IsNotNull("father")).Add(Restrictions.Ge("bodyWeight", 100f)).Add(Restrictions.IsEmpty("offspring"))).SetProjection(Projections.RowCount()).UniqueResult<int>();

				var deleteCount = session.CreateCriteria(typeof(Animal)).Add(Restrictions.Conjunction().Add(Restrictions.IsNotNull("father")).Add(Restrictions.Ge("bodyWeight", 100f)).Add(Restrictions.IsEmpty("offspring"))).Delete();

				var afterDeleteCount = session.CreateCriteria(typeof(Animal)).Add(Restrictions.Conjunction().Add(Restrictions.IsNotNull("father")).Add(Restrictions.Ge("bodyWeight", 100f)).Add(Restrictions.IsEmpty("offspring"))).SetProjection(Projections.RowCount()).UniqueResult<int>();

				Assert.AreEqual(beforeDeleteCount, deleteCount);
				Assert.AreEqual(0, afterDeleteCount);
			}
		}
	}
}
