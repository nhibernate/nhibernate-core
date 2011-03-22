using NUnit.Framework;
using SharpTestsEx;

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
		[Test]
		public void InheritanceSameColumnName()
		{
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var subClass = new SubClass();
				var referencing = new ReferencingClass() { SubClass = subClass };
				session.Save(subClass);
				session.Save(referencing);

				session.Transaction.Commit();
			}
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				var referencing = session.CreateQuery("from ReferencingClass")
					.UniqueResult<ReferencingClass>();

				// accessing a property of the base class to activate lazy loading
				// this line crashes because it tries to find the base class by
				// the wrong column name.
				BaseClass another;
				Executing.This(() => another = referencing.SubClass.Another).Should().NotThrow();

				session.Transaction.Commit();
			}
			using (var session = OpenSession())
			using (session.BeginTransaction())
			{
				session.CreateQuery("delete from ReferencingClass").ExecuteUpdate();
				session.CreateQuery("delete from BaseClass").ExecuteUpdate();
				session.Transaction.Commit();
			}

		}
	}
}