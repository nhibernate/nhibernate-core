using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Impl;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection
{
	/// <summary>
	/// Hibernate tests ported from trunk revision 19910 (July 8, 2010)
	/// </summary>
	public abstract class AbstractEntityWithManyToManyTest : TestCase
	{
		private bool isPlanContractsInverse;
		private bool isPlanContractsBidirectional;
		private bool isPlanVersioned;
		private bool isContractVersioned;

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}
		
		protected override void Configure(NHibernate.Cfg.Configuration configuration)
		{
			configuration.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true");
			configuration.SetProperty(NHibernate.Cfg.Environment.BatchSize, "0");
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			
			isPlanContractsInverse = Sfi.GetCollectionPersister(typeof(Plan).FullName + ".Contracts").IsInverse;

			try
			{
				Sfi.GetCollectionPersister(typeof(Contract).FullName + ".Plans");
				isPlanContractsBidirectional = true;
			}
			catch (MappingException)
			{
				isPlanContractsBidirectional = false;
			}

			isPlanVersioned = Sfi.GetEntityPersister(typeof(Plan).FullName).IsVersioned;
			isContractVersioned = Sfi.GetEntityPersister(typeof(Contract).FullName).IsVersioned;
		}
		
		[Test]
		public void UpdateProperty()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			p.AddContract(new Contract(null, "gail", "phone"));
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			p.Description = "new plan";
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			Contract c = p.Contracts.First();
			c.CustomerName = "yogi";
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(1));
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void CreateWithNonEmptyManyToManyCollectionOfNew()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			p.AddContract(new Contract(null, "gail", "phone"));
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			Contract c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(1));
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void CreateWithNonEmptyManyToManyCollectionOfExisting()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			Plan p = new Plan("plan");
			p.AddContract(c);
			s = OpenSession();
			t = s.BeginTransaction();
			s.Save(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(1));
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void AddNewManyToManyElementToPersistentEntity()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.Get<Plan>(p.Id);
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			p.AddContract(new Contract(null, "gail", "phone"));
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			Contract c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(1));
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void AddExistingManyToManyElementToPersistentEntity()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.Get<Plan>(p.Id);
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			c = s.Get<Contract>(c.Id);
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
			p.AddContract(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(isContractVersioned && isPlanVersioned ? 2 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void CreateWithEmptyManyToManyCollectionUpdateWithExistingElement()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			p.AddContract(c);
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(isContractVersioned && isPlanVersioned ? 2 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional) {
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void CreateWithNonEmptyManyToManyCollectionUpdateWithNewElement()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			Contract newC = new Contract(null, "sherman", "telepathy");
			p.AddContract(newC);
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(2));
			foreach (Contract aContract in p.Contracts)
			{
				if (aContract.Id == c.Id)
				{
					Assert.That(aContract.CustomerName, Is.EqualTo("gail"));
				}
				else if (aContract.Id == newC.Id)
				{
					Assert.That(aContract.CustomerName, Is.EqualTo("sherman"));
				}
				else
				{
					Assert.Fail("unknown contract");
				}
				if (isPlanContractsBidirectional)
				{
					Assert.That(aContract.Plans.First(), Is.SameAs(p));
				}
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void CreateWithEmptyManyToManyCollectionMergeWithExistingElement()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			p.AddContract(c);
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = (Plan)s.Merge(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(isContractVersioned && isPlanVersioned ? 2 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void CreateWithNonEmptyManyToManyCollectionMergeWithNewElement()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			Contract newC = new Contract(null, "yogi", "mail");
			p.AddContract(newC);
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = (Plan)s.Merge(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned && isPlanVersioned ? 1 : 0);  // NH-specific: Hibernate issues a separate UPDATE for the version number
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(2));
			foreach (Contract aContract in p.Contracts)
			{
				if (aContract.Id == c.Id)
				{
					Assert.That(aContract.CustomerName, Is.EqualTo("gail"));
				}
				else if (!aContract.CustomerName.Equals(newC.CustomerName))
				{
					Assert.Fail("unknown contract:" + aContract.CustomerName);
				}
				if (isPlanContractsBidirectional)
				{
					Assert.That(aContract.Plans.First(), Is.SameAs(p));
				}
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void RemoveManyToManyElementUsingUpdate()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			p.RemoveContract(c);
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(p);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			if (isPlanContractsInverse)
			{
				Assert.That(p.Contracts.Count, Is.EqualTo(1));
				c = p.Contracts.First();
				Assert.That(c.CustomerName, Is.EqualTo("gail"));
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			else
			{
				Assert.That(p.Contracts.Count, Is.EqualTo(0));
				c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
				if (isPlanContractsBidirectional)
				{
					Assert.That(c.Plans.Count, Is.EqualTo(0));
				}
				s.Delete(c);
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void RemoveManyToManyElementUsingUpdateBothSides()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			p.RemoveContract(c);
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(p);
			s.Update(c);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned && isPlanVersioned ? 2 : 0);
			AssertDeleteCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isPlanContractsBidirectional) {
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
			s.Delete(c);
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void RemoveManyToManyElementUsingMerge()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			p.RemoveContract(c);
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			if (isPlanContractsBidirectional) {
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
			s = OpenSession();
			t = s.BeginTransaction();
			p = (Plan)s.Merge(p);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			if (isPlanContractsInverse) {
				Assert.That(p.Contracts.Count, Is.EqualTo(1));
				c = p.Contracts.First();
				Assert.That(c.CustomerName, Is.EqualTo("gail"));
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			else {
				Assert.That(p.Contracts.Count, Is.EqualTo(0));
				c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
				if (isPlanContractsBidirectional) {
					Assert.That(c.Plans.Count, Is.EqualTo(0));
				}
				s.Delete(c);
			}
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void RemoveManyToManyElementUsingMergeBothSides()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			p.RemoveContract(c);
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = (Plan)s.Merge(p);
			c = (Contract)s.Merge(c);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned  && isPlanVersioned ? 2 : 0);
			AssertDeleteCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
			s.Delete(c);
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public void DeleteManyToManyElement()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(p);
			p.RemoveContract(c);
			s.Delete(c);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c, Is.Null);
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void RemoveManyToManyElementByDelete()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			Contract c = new Contract(null, "gail", "phone");
			p.AddContract(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			p.RemoveContract(c);
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.Count, Is.EqualTo(0));
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(p);
			s.Delete(c);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isPlanVersioned ? 1 : 0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(0));
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public void ManyToManyCollectionOptimisticLockingWithMerge()
		{
			ClearCounts();
	
			Plan pOrig = new Plan("plan");
			Contract cOrig = new Contract(null, "gail", "phone");
			pOrig.AddContract(cOrig);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(pOrig);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Plan p = s.Get<Plan>(pOrig.Id);
			Contract newC = new Contract(null, "sherman", "note");
			p.AddContract(newC);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			pOrig.RemoveContract(cOrig);
			try
			{
				s.Merge(pOrig);
				Assert.That(isContractVersioned, Is.False);
			}
			catch (StaleObjectStateException)
			{
				Assert.That(isContractVersioned, Is.True);
			}
			finally
			{
				t.Rollback();
			}
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			s.Delete(p);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void ManyToManyCollectionOptimisticLockingWithUpdate()
		{
			ClearCounts();
	
			Plan pOrig = new Plan("plan");
			Contract cOrig = new Contract(null, "gail", "phone");
			pOrig.AddContract(cOrig);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(pOrig);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Plan p = s.Get<Plan>(pOrig.Id);
			Contract newC = new Contract(null, "yogi", "pawprint");
			p.AddContract(newC);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			pOrig.RemoveContract(cOrig);
			s.Update(pOrig);
			try
			{
				t.Commit();
				Assert.That(isContractVersioned, Is.False);
			}
			catch (StaleObjectStateException)
			{
				Assert.That(isContractVersioned, Is.True);
				t.Rollback();
			}
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			s.Delete(p);
			s.CreateQuery("delete from Contract").ExecuteUpdate();
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void MoveManyToManyElementToNewEntityCollection()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			p.AddContract(new Contract(null, "gail", "phone"));
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			Contract c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			p.RemoveContract(c);
			Plan p2 = new Plan("new plan");
			p2.AddContract(c);
			s.Save(p2);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isPlanVersioned && isContractVersioned ? 2 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().Add(Restrictions.IdEq(p.Id)).UniqueResult<Plan>();
			p2 = s.CreateCriteria<Plan>().Add(Restrictions.IdEq(p2.Id)).UniqueResult<Plan>();
			/*
			if (isPlanContractsInverse) {
				Assert.That(p.Contracts.Count, Is.EqualTo(1));
				c = p.Contracts.First();
				Assert.That(c.CustomerName, Is.EqualTo("gail"));
				if (isPlanContractsBidirectional) {
					Assert.That(c.Plans.First(), Is.SameAs(p));
				}
				assertEquals( 0, p2.getContracts().size() );
			}
			else {
			*/
				Assert.That(p.Contracts.Count, Is.EqualTo(0));
				Assert.That(p2.Contracts.Count, Is.EqualTo(1));
				c = p2.Contracts.First();
				Assert.That(c.CustomerName, Is.EqualTo("gail"));
				if (isPlanContractsBidirectional)
				{
					Assert.That(c.Plans.First(), Is.SameAs(p2));
				}
			//}
			s.Delete(p);
			s.Delete(p2);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public void MoveManyToManyElementToExistingEntityCollection()
		{
			ClearCounts();
	
			Plan p = new Plan("plan");
			p.AddContract(new Contract(null, "gail", "phone"));
			Plan p2 = new Plan("plan2");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(p);
			s.Persist(p2);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().Add(Restrictions.IdEq(p.Id)).UniqueResult<Plan>();
			Assert.That(p.Contracts.Count, Is.EqualTo(1));
			Contract c = p.Contracts.First();
			Assert.That(c.CustomerName, Is.EqualTo("gail"));
			if (isPlanContractsBidirectional)
			{
				Assert.That(c.Plans.First(), Is.SameAs(p));
			}
			p.RemoveContract(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(isPlanVersioned && isContractVersioned ? 2 : 0);
			ClearCounts();
			
			s = OpenSession();
			t = s.BeginTransaction();
			p2 = s.CreateCriteria<Plan>().Add(Restrictions.IdEq(p2.Id)).UniqueResult<Plan>();
			c = s.CreateCriteria<Contract>().Add(Restrictions.IdEq(c.Id)).UniqueResult<Contract>();
			p2.AddContract(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(isPlanVersioned && isContractVersioned ? 2 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			p = s.CreateCriteria<Plan>().Add(Restrictions.IdEq(p.Id)).UniqueResult<Plan>();
			p2 = s.CreateCriteria<Plan>().Add(Restrictions.IdEq(p2.Id)).UniqueResult<Plan>();
			/*
			if (isPlanContractsInverse) {
				Assert.That(p.Contracts.Count, Is.EqualTo(1));
				c = p.Contracts.First();
				Assert.That(c.CustomerName, Is.EqualTo("gail"));
				if (isPlanContractsBidirectional) {
					Assert.That(c.Plans.First(), Is.SameAs(p));
				}
				assertEquals( 0, p2.getContracts().size() );
			}
			else {
			*/
				Assert.That(p.Contracts.Count, Is.EqualTo(0));
				Assert.That(p2.Contracts.Count, Is.EqualTo(1));
				c = p2.Contracts.First();
				Assert.That(c.CustomerName, Is.EqualTo("gail"));
				if (isPlanContractsBidirectional)
				{
					Assert.That(c.Plans.First(), Is.SameAs(p2));
				}
			//}
			s.Delete(p);
			s.Delete(p2);
			Assert.That(s.CreateCriteria<Plan>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
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
			Assert.That(Sfi.Statistics.EntityInsertCount, Is.EqualTo(count), "unexpected insert count");
		}

		protected void AssertDeleteCount(int count)
		{
			Assert.That(Sfi.Statistics.EntityDeleteCount, Is.EqualTo(count), "unexpected delete counts");
		}
	}
}
