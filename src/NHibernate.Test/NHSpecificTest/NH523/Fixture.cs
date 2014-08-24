using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH523
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH523"; }
		}

		[Test]
		public void MergeLazy()
		{
			ClassA a = new ClassA();
			a.B = new ClassB();

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(a);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Merge(a);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Delete(a);
				s.Delete(a.B);
				t.Commit();
			}
		}
	}
}