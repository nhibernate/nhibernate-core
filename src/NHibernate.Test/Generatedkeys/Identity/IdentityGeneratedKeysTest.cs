using System.Collections;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Test.Generatedkeys.Identity
{
	[TestFixture]
	public class IdentityGeneratedKeysTest : TestCase
	{
		protected override IList Mappings
		{
			get { return new string[] { "Generatedkeys.Identity.MyEntity.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect.SupportsIdentityColumns;
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(Environment.GenerateStatistics, "true");
		}

		[Test]
		public void IdentityColumnGeneratedIds()
		{
			ISession s = OpenSession();
			s.BeginTransaction();
			MyEntity myEntity = new MyEntity("test");
			long id = (long)s.Save(myEntity);
			Assert.IsNotNull(id,"identity column did not force immediate insert");
			Assert.AreEqual(id, myEntity.Id);
			s.Delete(myEntity);
			s.Transaction.Commit();
			s.Close();
		}

		[Test, Ignore("Not supported yet.")]
		public void PersistOutsideTransaction()
		{
			ISession s = OpenSession();

			// first test save() which should force an immediate insert...
			MyEntity myEntity1 = new MyEntity("test-save");
			long id = (long)s.Save(myEntity1);
			Assert.IsNotNull(id, "identity column did not force immediate insert");
			Assert.AreEqual(id, myEntity1.Id);

			// next test persist() which should cause a delayed insert...
			long initialInsertCount = Sfi.Statistics.EntityInsertCount;
			MyEntity myEntity2 = new MyEntity("test-persist");
			s.Persist(myEntity2);
			Assert.AreEqual(initialInsertCount, Sfi.Statistics.EntityInsertCount, "persist on identity column not delayed");
			Assert.AreEqual(0,myEntity2.Id);

			// an explicit flush should cause execution of the delayed insertion
			s.Flush();
			Assert.AreEqual(initialInsertCount + 1, Sfi.Statistics.EntityInsertCount, "delayed persist insert not executed on flush");
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete(myEntity1);
			s.Delete(myEntity2);
			s.Transaction.Commit();
			s.Close();
		}

		[Test, Ignore("Not supported yet.")]
		public void PersistOutsideTransactionCascadedToNonInverseCollection()
		{
			long initialInsertCount = Sfi.Statistics.EntityInsertCount;
			ISession s = OpenSession();
			MyEntity myEntity = new MyEntity("test-persist");
			myEntity.NonInverseChildren.Add(new MyChild("test-child-persist-non-inverse"));
			s.Persist(myEntity);
			Assert.AreEqual(initialInsertCount, Sfi.Statistics.EntityInsertCount, "persist on identity column not delayed");
			Assert.AreEqual(0, myEntity.Id);
			s.Flush();
			Assert.AreEqual(initialInsertCount + 2, Sfi.Statistics.EntityInsertCount,"delayed persist insert not executed on flush");
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete("from MyChild");
			s.Delete("from MyEntity");
			s.Transaction.Commit();
			s.Close();
		}

		[Test, Ignore("Not supported yet.")]
		public void PersistOutsideTransactionCascadedToInverseCollection()
		{
			long initialInsertCount = Sfi.Statistics.EntityInsertCount;
			ISession s = OpenSession();
			MyEntity myEntity2 = new MyEntity("test-persist-2");
			MyChild child = new MyChild("test-child-persist-inverse");
			myEntity2.InverseChildren.Add(child);
			child.InverseParent= myEntity2;
			s.Persist(myEntity2);
			Assert.AreEqual(initialInsertCount, Sfi.Statistics.EntityInsertCount,"persist on identity column not delayed");
			Assert.AreEqual(0, myEntity2.Id);
			s.Flush();
			Assert.AreEqual(initialInsertCount + 2, Sfi.Statistics.EntityInsertCount,"delayed persist insert not executed on flush");
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete("from MyChild");
			s.Delete("from MyEntity");
			s.Transaction.Commit();
			s.Close();
		}

		[Test, Ignore("Not supported yet.")]
		public void PersistOutsideTransactionCascadedToManyToOne()
		{
			long initialInsertCount = Sfi.Statistics.EntityInsertCount;
			ISession s = OpenSession();
			MyEntity myEntity = new MyEntity("test-persist");
			myEntity.Sibling=new MySibling("test-persist-sibling-out");
			s.Persist(myEntity);
			Assert.AreEqual(initialInsertCount, Sfi.Statistics.EntityInsertCount, "persist on identity column not delayed");
			Assert.AreEqual(0, myEntity.Id);
			s.Flush();
			Assert.AreEqual(initialInsertCount + 2, Sfi.Statistics.EntityInsertCount, "delayed persist insert not executed on flush");
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete("from MyEntity");
			s.Delete("from MySibling");
			s.Transaction.Commit();
			s.Close();
		}

		[Test, Ignore("Not supported yet.")]
		public void PersistOutsideTransactionCascadedFromManyToOne()
		{
			long initialInsertCount = Sfi.Statistics.EntityInsertCount;
			ISession s = OpenSession();
			MyEntity myEntity2 = new MyEntity("test-persist-2");
			MySibling sibling = new MySibling("test-persist-sibling-in");
			sibling.Entity= myEntity2;
			s.Persist(sibling);
			Assert.AreEqual(initialInsertCount, Sfi.Statistics.EntityInsertCount, "persist on identity column not delayed");
			Assert.AreEqual(0, myEntity2.Id);
			s.Flush();
			Assert.AreEqual(initialInsertCount + 2, Sfi.Statistics.EntityInsertCount, "delayed persist insert not executed on flush");
			s.Close();

			s = OpenSession();
			s.BeginTransaction();
			s.Delete("from MySibling");
			s.Delete("from MyEntity");
			s.Transaction.Commit();
			s.Close();
		}
	}
}
