using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Test.Immutable
{
	/// <summary>
	/// Hibernate tests ported from trunk revision 20154 (August 17, 2010)
	/// </summary>
	[TestFixture]
	public class ImmutableTest : TestCase
	{
		protected override void Configure(Configuration configuration)
		{
			base.Configure(configuration);
			configuration.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true");
			configuration.SetProperty(NHibernate.Cfg.Environment.BatchSize, "0");
		}
		
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}
		
		protected override IList Mappings
		{
			get { return new string[] { "Immutable.ContractVariation.hbm.xml" }; }
		}
	
		[Test]
		public void ChangeImmutableEntityProxyToModifiable()
		{
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
	
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
	
			try
			{
				Assert.That(c, Is.InstanceOf<INHibernateProxy>());
				s.SetReadOnly(c, false);
			}
			catch (System.InvalidOperationException)
			{
				// expected
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}

		[Test]
		public void ChangeImmutableEntityToModifiable()
		{
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
	
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
	
			try
			{
				Assert.That(c, Is.InstanceOf<INHibernateProxy>());
				s.SetReadOnly(((INHibernateProxy)c).HibernateLazyInitializer.GetImplementation(), false);
			}
			catch (System.InvalidOperationException)
			{
				// expected
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}

		[Test]
		public void PersistImmutable()
		{
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
	
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}

		[Test]
		public void PersistUpdateImmutableInSameTransaction()
		{
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
	
			ClearCounts();
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			c.CustomerName = "gail";
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void SaveImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Save(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void SaveOrUpdateImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.SaveOrUpdate(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void RefreshImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.SaveOrUpdate(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			// refresh detached
			s.Refresh(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.CustomerName = "joe";
	
			s = OpenSession();
			t = s.BeginTransaction();
			// refresh updated detached
			s.Refresh(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void Immutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			c.CustomerName = "foo bar";
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			cv1.Text = "blah blah";
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.Contains(cv2), Is.False);
			t.Commit();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.Contains(cv2), Is.False);
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void PersistAndUpdateImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			c.CustomerName = "Sherman";
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			c.CustomerName = "foo bar";
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			cv1.Text = "blah blah";
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.Contains(cv2), Is.False);
			t.Commit();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.Contains(cv2), Is.False);
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void UpdateAndDeleteManagedImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			c.CustomerName = "Sherman";
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}

		[Test]
		public void GetAndDeleteManagedImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.Get<Contract>(c.Id);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			c.CustomerName = "Sherman";
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void DeleteDetachedImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(c);
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c, Is.Null);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void DeleteDetachedModifiedImmutable()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c.CustomerName = "Sherman";
			s.Delete(c);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void ImmutableParentEntityWithUpdate()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c.CustomerName = "foo bar";
			s.Update(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			foreach(ContractVariation variation in c.Variations)
			{
				Assert.That(s.Contains(variation), Is.True);
			}
			t.Commit();
			Assert.That(s.IsReadOnly(c), Is.True);
			foreach(ContractVariation variation in c.Variations)
			{
				Assert.That(s.Contains(variation), Is.True);
				Assert.That(s.IsReadOnly(variation), Is.True);
			}
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void ImmutableChildEntityWithUpdate()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cv1 = c.Variations.First();
			cv1.Text = "blah blah";
			s.Update(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.Contains(cv1), Is.True);
			Assert.That(s.Contains(cv2), Is.True);
			t.Commit();
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void ImmutableCollectionWithUpdate()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c.Variations.Add(new ContractVariation(3, c));
			s.Update(c);
			try
			{
				t.Commit();
				Assert.Fail( "should have failed because reassociated object has a dirty collection");
			}
			catch (HibernateException)
			{
				// expected
			}
			finally
			{
				t.Rollback();
				s.Close();
			}
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void UnmodifiedImmutableParentEntityWithMerge()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = (Contract)s.Merge(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(c.Variations), Is.True);
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(s.IsReadOnly(cv1), Is.True);
			Assert.That(s.IsReadOnly(cv2), Is.True);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void ImmutableParentEntityWithMerge()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c.CustomerName = "foo bar";
			c = (Contract)s.Merge(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(c.Variations), Is.True);
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(c), Is.True);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
	
		}
	
		[Test]
		public void ImmutableChildEntityWithMerge()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cv1 = c.Variations.First();
			cv1.Text = "blah blah";
			c = (Contract)s.Merge(c);
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(c.Variations), Is.True);
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(s.IsReadOnly(c), Is.True);
			Assert.That(s.IsReadOnly(c), Is.True);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void ImmutableCollectionWithMerge()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
	
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c.Variations.Add(new ContractVariation(3, c));
			s.Merge(c);
			try
			{
				t.Commit();
				Assert.Fail("should have failed because an immutable collection was changed");
			}
			catch ( HibernateException )
			{
				// expected
				t.Rollback();
			}
			finally
			{
				s.Close();
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void NewEntityViaImmutableEntityWithImmutableCollectionUsingSaveOrUpdate()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cv1.Infos.Add(new Info("cv1 info"));
			s.SaveOrUpdate(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			Assert.That(cv1.Infos.Count, Is.EqualTo(1));
			Assert.That(cv1.Infos.First().Text, Is.EqualTo("cv1 info"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(4);
		}
	
		[Test]
		public void NewEntityViaImmutableEntityWithImmutableCollectionUsingMerge()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cv1.Infos.Add(new Info("cv1 info"));
			s.Merge(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			Assert.That(cv1.Infos.Count, Is.EqualTo(1));
			Assert.That(cv1.Infos.First().Text, Is.EqualTo("cv1 info"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(4);
		}
	
		[Test]
		public void UpdatedEntityViaImmutableEntityWithImmutableCollectionUsingSaveOrUpdate()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			Info cv1Info = new Info( "cv1 info" );
			cv1.Infos.Add(cv1Info);
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cv1Info.Text = "new cv1 info";
			s.SaveOrUpdate(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			Assert.That(cv1.Infos.Count, Is.EqualTo(1));
			Assert.That(cv1.Infos.First().Text, Is.EqualTo("new cv1 info"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(4);
		}
	
		[Test]
		public void UpdatedEntityViaImmutableEntityWithImmutableCollectionUsingMerge()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			Info cv1Info = new Info( "cv1 info" );
			cv1.Infos.Add(cv1Info);
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cv1Info.Text = "new cv1 info";
			s.Merge(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			Assert.That(cv1.Infos.Count, Is.EqualTo(1));
			Assert.That(cv1.Infos.First().Text, Is.EqualTo("new cv1 info"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(4);
		}
	
		[Test]
		public void ImmutableEntityAddImmutableToInverseMutableCollection()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			Party party = new Party("a party");
			s.Persist(party);
			t.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c.AddParty(new Party("a new party"));
			s.Update(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c.AddParty(party);
			s.Update(c);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			// Assert.That(c.Parties.Count, Is.EqualTo(2));
			s.Delete(c);
			s.Delete(party); // NH-specific
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(5); // NH-specific
		}
		
		[Test]
		public void ImmutableEntityRemoveImmutableFromInverseMutableCollection()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			Party party = new Party( "party1" );
			c.AddParty(party);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
			ClearCounts();
	
			party = c.Parties.First();
			c.RemoveParty(party);
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			//Assert.That(c.Parties.Count, Is.EqualTo(0));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(4);
		}
	
		[Test]
		public void ImmutableEntityRemoveImmutableFromInverseMutableCollectionByDelete()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			Party party = new Party( "party1" );
			c.AddParty(party);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
			ClearCounts();
	
			party = c.Parties.First();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Delete(party);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void ImmutableEntityRemoveImmutableFromInverseMutableCollectionByDeref()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gavin", "phone");
			ContractVariation cv1 = new ContractVariation(1, c);
			cv1.Text = "expensive";
			ContractVariation cv2 = new ContractVariation(2, c);
			cv2.Text = "more expensive";
			Party party = new Party( "party1" );
			c.AddParty(party);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(4);
			AssertUpdateCount(0);
			ClearCounts();
	
			party = c.Parties.First();
			party.Contract = null;
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(party);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			party = s.Get<Party>(party.Id);
			Assert.That(party.Contract, Is.Not.Null);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.CustomerName, Is.EqualTo("gavin"));
			Assert.That(c.Variations.Count, Is.EqualTo(2));
			IEnumerator<ContractVariation> it = c.Variations.GetEnumerator();
			it.MoveNext();
			cv1 = it.Current;
			Assert.That(cv1.Text, Is.EqualTo("expensive"));
			it.MoveNext();
			cv2 = it.Current;
			Assert.That(cv2.Text, Is.EqualTo("more expensive"));
			Assert.That(c.Parties.Count, Is.EqualTo(1));
		    party = c.Parties.First();
			Assert.That(party.Name, Is.EqualTo("party1"));
			Assert.That(party.Contract, Is.SameAs(c));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCount()).UniqueResult(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(4);
		}
		
		protected void ClearCounts()
		{
			Sfi.Statistics.Clear();
		}
		
		protected void AssertUpdateCount(int count)
		{
			Assert.That(Sfi.Statistics.EntityUpdateCount, Is.EqualTo(count), "unexpected update counts");
		}
		
		protected void AssertInsertCount(int count)
		{
			Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(count), "unexpected insert counts");
		}

		protected void AssertDeleteCount(int count)
		{
			Assert.That(Sfi.Statistics.EntityDeleteCount, Is.EqualTo(count), "unexpected delete counts");
		}
	}
}
