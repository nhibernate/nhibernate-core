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
			int catId;

			using (ISession sess = OpenSession())
			{
				Cat c = new Cat();
				sess.Save(c);
				catId = c.Id;
				sess.Flush();
			}

			using (ISession sess = OpenSession())
			{
				Cat c = (Cat) sess.Get(typeof(Cat), catId);
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