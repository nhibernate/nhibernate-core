using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH704
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH704"; }
		}

		[Test]
		public void ReAttachCatTest()
		{
			try
			{
				using (ISession sess = OpenSession())
				{
					Cat c = new Cat();
					sess.Save(c);
					sess.Flush();
					sess.Clear();
					sess.Lock(c, LockMode.None); //Exception throw here
				}
			}
			finally
			{
				using (ISession s = OpenSession())
				{
					s.Delete("from Cat");
					s.Flush();
				}
			}
		}
	}
}