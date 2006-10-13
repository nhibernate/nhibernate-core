using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH739
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void Bug()
		{
			using (ISession sess = OpenSession())
			{
				Cat c = new Cat();
				sess.Save(c);
				sess.Clear();
				c = (Cat) sess.Get(typeof(Cat), c.Id);
				Cat kitten = new Cat();
				c.Children.Add(kitten);
				kitten.Mother = c;
				sess.Save(kitten);

				Assert.AreEqual(1, c.Children.Count); //Test will fail here, the c.Children.Count is 2 here

				sess.Delete(c);
				sess.Delete(kitten);
				sess.Flush();
			}
		}
	}
}
