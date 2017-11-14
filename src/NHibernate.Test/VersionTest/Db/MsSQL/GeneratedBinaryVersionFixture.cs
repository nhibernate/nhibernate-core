using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.VersionTest.Db.MsSQL
{
	// related issues NH-1687, NH-1685

	[TestFixture]
	public class GeneratedBinaryVersionFixture : TestCase
	{
		protected override IList Mappings
		{
			get { return new[] { "VersionTest.Db.MsSQL.SimpleVersioned.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}

		[Test]
		public void ShouldRetrieveVersionAfterFlush()
		{
			// Note : if you are using identity-style strategy the value of version
			// is available inmediately after save.
			var e = new SimpleVersioned {Something = "something"};
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					Assert.That(e.LastModified, Is.Null);
					s.Save(e);
					s.Flush();
					Assert.That(e.LastModified, Is.Not.Null);
					s.Delete(e);
					tx.Commit();
				}
			}
		}

		[Test]
		public void ShouldChangeAfterUpdate()
		{
			object savedId = PersistANewSomething();
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var fetched = s.Get<SimpleVersioned>(savedId);
					var freshVersion = fetched.LastModified;
					fetched.Something = "make it dirty";
					s.Update(fetched);
					s.Flush(); // force flush to hit DB
					Assert.That(fetched.LastModified, Is.Not.SameAs(freshVersion));
					s.Delete(fetched);
					tx.Commit();
				}
			}
		}

		private object PersistANewSomething()
		{
			object savedId;
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var e = new SimpleVersioned {Something = "something"};
					savedId = s.Save(e);
					tx.Commit();
				}
			}
			return savedId;
		}

		[Test]
		public void ShouldCheckStaleState()
		{
			var versioned = new SimpleVersioned {Something = "original string"};

			try
			{
				using (var session = OpenSession())
				{
					session.Save(versioned);
					session.Flush();

					using (var concurrentSession = OpenSession())
					{
						var sameVersioned = concurrentSession.Get<SimpleVersioned>(versioned.Id);
						sameVersioned.Something = "another string";
						concurrentSession.Flush();
					}

					versioned.Something = "new string";

					var expectedException = Sfi.Settings.IsBatchVersionedDataEnabled
						? Throws.InstanceOf<StaleStateException>()
						: Throws.InstanceOf<StaleObjectStateException>();

					Assert.That(() => session.Flush(), expectedException);
				}
			}
			finally
			{
				using (ISession session = OpenSession())
				{
					session.Delete("from SimpleVersioned");
					session.Flush();
				}
			}
		}
	}
}