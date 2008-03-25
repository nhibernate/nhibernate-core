using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH525
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH525"; }
		}

		[Test]
		public void DoSomething()
		{
			NonAbstract obj = new NonAbstract();

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(obj);
				t.Commit();
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				AbstractBase baseObj = (AbstractBase) s.Load(typeof(AbstractBase), obj.Id);
				Assert.AreEqual(NonAbstract.AbstractMethodResult, baseObj.AbstractMethod());
				s.Delete(baseObj);
				t.Commit();
			}
		}
	}
}