using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2491
{
	public class BaseClass
	{
		public virtual int Id { get; set; }

		public virtual BaseClass Another { get; set; }
	}

	public class SubClass : BaseClass
	{
	}

	public class ReferencingClass
	{
		public virtual int Id { get; set; }

		public virtual SubClass SubClass { get; set; }
	}


	public class Fixture : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				tx.Commit();
			}
		}

		[Test]
		public void InheritanceSameColumnName()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var subClass = new SubClass();
				var referencing = new ReferencingClass() { SubClass = subClass };
				session.Save(subClass);
				session.Save(referencing);

				transaction.Commit();
			}
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var referencing = session.CreateQuery("from ReferencingClass")
					.UniqueResult<ReferencingClass>();

				// accessing a property of the base class to activate lazy loading
				// this line crashes because it tries to find the base class by
				// the wrong column name.
				BaseClass another;
				Assert.That(() => another = referencing.SubClass.Another, Throws.Nothing);

				transaction.Commit();
			}
		}
	}
}