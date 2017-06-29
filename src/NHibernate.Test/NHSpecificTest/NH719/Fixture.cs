using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH719
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH719"; }
		}

		[Test]
		public void CacheLoadTest()
		{
			//Instantiate and setup associations (all needed to generate the error);
			A a = new A(1, "aaa");
			B b = new B(2, "bbb");
			NotCached notCached = new NotCached(1, b);
			Cached cached = new Cached(1, a);

			try
			{
				using (ISession session = Sfi.OpenSession())
				{
					session.Save(a);
					session.Save(b);
					session.Save(notCached);
					session.Save(cached);

					session.Flush();
				}

				using (ISession session = Sfi.OpenSession())
				{
					// runs OK, since it's not cached
					NotCached nc = (NotCached) session.Load(typeof(NotCached), 1);
					Assert.AreEqual("bbb", ((B) nc.Owner).Foo);

					// 1st run OK, not yet in cache
					Cached ca = (Cached) session.Load(typeof(Cached), 1);
					Assert.AreEqual("aaa", ((A) ca.Owner).Foo);
				}

				// 2nd run fails, when data is read from the cache
				using (ISession session = Sfi.OpenSession())
				{
					// runs OK, since it's not cached
					NotCached nc = (NotCached) session.Load(typeof(NotCached), 1);
					Assert.AreEqual("bbb", ((B) nc.Owner).Foo);

					// 2nd run fails, when loaded from in cache
					Cached ca = (Cached) session.Load(typeof(Cached), 1);
					Assert.AreEqual("aaa", ((A) ca.Owner).Foo);
				}
			}
			finally
			{
				using (ISession session = OpenSession())
				{
					session.Delete(notCached);
					session.Delete(cached);
					session.Delete(a);
					session.Delete(b);
					session.Flush();
				}
			}
		}
	}
}