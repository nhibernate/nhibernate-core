using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1069
{
	[TestFixture]
	public class ImproveLazyExceptionFixture: BugTestCase
	{
		[Test]
		public void LazyEntity()
		{
			object savedId = 1;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new LazyE(), 1);
				t.Commit();
			}

			LazyE le;
			using (ISession s = OpenSession())
			{
				le = s.Load<LazyE>(savedId);
			}
			string n;
			var ex = Assert.Throws<LazyInitializationException>(() => n= le.Name);
			Assert.That(ex.EntityName, Is.EqualTo(typeof (LazyE).FullName));
			Assert.That(ex.EntityId, Is.EqualTo(1));
			Assert.That(ex.Message, Is.StringContaining(typeof(LazyE).FullName));
			Assert.That(ex.Message, Is.StringContaining("#1"));
			Console.WriteLine(ex.Message);

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from LazyE").ExecuteUpdate();
				t.Commit();
			}
		}

		[Test]
		public void LazyCollection()
		{
			object savedId=1;
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.Save(new LazyE(), savedId);
				t.Commit();
			}

			LazyE le;
			using (ISession s = OpenSession())
			{
				le = s.Get<LazyE>(savedId);
			}
			var ex = Assert.Throws<LazyInitializationException>(() => le.LazyC.GetEnumerator());
			Assert.That(ex.EntityName, Is.EqualTo(typeof(LazyE).FullName));
			Assert.That(ex.EntityId, Is.EqualTo(1));
			Assert.That(ex.Message, Is.StringContaining(typeof(LazyE).FullName));
			Assert.That(ex.Message, Is.StringContaining("#1"));
			Assert.That(ex.Message, Is.StringContaining(typeof(LazyE).FullName + ".LazyC"));

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				s.CreateQuery("delete from LazyE").ExecuteUpdate();
				t.Commit();
			}
		}
	}
}