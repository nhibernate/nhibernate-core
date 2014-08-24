using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH883
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

				// Double flush
				sess.Flush();
				sess.Flush();

				sess.Delete(c);
				sess.Delete(kitten);
				sess.Flush();
			}
		}
	}
}