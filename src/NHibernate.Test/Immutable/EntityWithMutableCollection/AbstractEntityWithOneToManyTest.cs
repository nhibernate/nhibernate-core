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
	public abstract class AbstractEntityWithOneToManyTest : TestCase
	{
		private bool isContractPartiesInverse;
		private bool isContractPartiesBidirectional;
		private bool isContractVariationsBidirectional;
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
	
		protected virtual bool CheckUpdateCountsAfterAddingExistingElement()
		{
			return true;
		}
	
		protected virtual bool CheckUpdateCountsAfterRemovingElementWithoutDelete()
		{
			return true;
		}
	
		protected override void OnSetUp()
		{
			isContractPartiesInverse = Sfi.GetCollectionPersister(typeof(Contract).FullName + ".Parties").IsInverse;
			try
			{
				Sfi.GetEntityPersister(typeof(Party).FullName).GetPropertyType("Contract");
				isContractPartiesBidirectional = true;
			}
			catch (QueryException)
			{
				isContractPartiesBidirectional = false;
			}
			try
			{
				Sfi.GetEntityPersister(typeof(ContractVariation).FullName).GetPropertyType("Contract");
				isContractVariationsBidirectional = true;
			}
			catch (QueryException)
			{
				isContractVariationsBidirectional = false;
			}
	
			isContractVersioned = Sfi.GetEntityPersister(typeof(Contract).FullName).IsVersioned;
		}
		
		[Test]
		public virtual void UpdateProperty()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			c.AddParty(new Party("party"));
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			c.CustomerName = "yogi";
			Assert.That(c.Parties.Count, Is.EqualTo(1));
			Party party = c.Parties.First();
			party.Name = "new party";
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(1));
			party = c.Parties.First();
			Assert.That(party.Name, Is.EqualTo("party"));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.SameAs(c));
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void CreateWithNonEmptyOneToManyCollectionOfNew()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			c.AddParty(new Party("party"));
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(1));
			Party party = c.Parties.First();
			Assert.That(party.Name, Is.EqualTo("party"));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.SameAs(c));
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void CreateWithNonEmptyOneToManyCollectionOfExisting()
		{
			ClearCounts();
	
			Party party = new Party("party");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(party);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(0);
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			c.AddParty(party);
			s = OpenSession();
			t = s.BeginTransaction();
			s.Save(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			// BUG, should be assertUpdateCount( ! isContractPartiesInverse && isPartyVersioned ? 1 : 0 );
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				party = s.CreateCriteria<Party>().UniqueResult<Party>();
				Assert.That(party.Contract, Is.Null);
				s.Delete(party);
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c));
				}
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void AddNewOneToManyElementToPersistentEntity()
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
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.Get<Contract>(c.Id);
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			c.AddParty(new Party("party"));
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(1));
			Party party = c.Parties.First();
			Assert.That(party.Name, Is.EqualTo("party"));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.SameAs(c));
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void AddExistingOneToManyElementToPersistentEntity()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			s.Persist(party);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.Get<Contract>(c.Id);
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			party = s.Get<Party>(party.Id);
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.Null);
			}
			c.AddParty(party);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			if (CheckUpdateCountsAfterAddingExistingElement())
			{
				AssertUpdateCount(isContractVersioned && !isContractPartiesInverse ? 1 : 0);
			}
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				s.Delete(party);
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c));
				}
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
		
		[Test]
		public virtual void CreateWithEmptyOneToManyCollectionUpdateWithExistingElement()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			s.Persist(party);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.AddParty(party);
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			if (CheckUpdateCountsAfterAddingExistingElement())
			{
				AssertUpdateCount(isContractVersioned && !isContractPartiesInverse ? 1 : 0);
			}
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				s.Delete(party);
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c));
				}
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void CreateWithNonEmptyOneToManyCollectionUpdateWithNewElement()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
			c.AddParty(party);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			Party newParty = new Party("new party");
			c.AddParty(newParty);
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(2));
			foreach (Party aParty in c.Parties)
			{
				if (aParty.Id == party.Id)
				{
					Assert.That(aParty.Name, Is.EqualTo("party"));
				}
				else if (aParty.Id == newParty.Id)
				{
					Assert.That(aParty.Name, Is.EqualTo("new party"));
				}
				else
				{
					Assert.Fail("unknown party");
				}
				if (isContractPartiesBidirectional)
				{
					Assert.That(aParty.Contract, Is.SameAs(c));
				}
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public virtual void CreateWithEmptyOneToManyCollectionMergeWithExistingElement()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			s.Persist(party);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.AddParty(party);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = (Contract)s.Merge(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			if (CheckUpdateCountsAfterAddingExistingElement())
			{
				AssertUpdateCount(isContractVersioned && !isContractPartiesInverse ? 1 : 0);
			}
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				s.Delete(party);
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c));
				}
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void CreateWithNonEmptyOneToManyCollectionMergeWithNewElement()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
			c.AddParty(party);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			Party newParty = new Party("new party");
			c.AddParty(newParty);
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = (Contract)s.Merge(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(2));
			foreach (Party aParty in c.Parties)
			{
				if (aParty.Id == party.Id)
				{
					Assert.That(aParty.Name, Is.EqualTo("party"));
				}
				else if (!aParty.Name.Equals(newParty.Name))
				{
					Assert.Fail("unknown party:" + aParty.Name);
				}
				if (isContractPartiesBidirectional)
				{
					Assert.That(aParty.Contract, Is.SameAs(c));
				}
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public virtual void MoveOneToManyElementToNewEntityCollection()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			c.AddParty(new Party("party"));
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(1));
			Party party = c.Parties.First();
			Assert.That(party.Name, Is.EqualTo("party"));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.SameAs(c));
			}
			c.RemoveParty(party);
			Contract c2 = new Contract(null, "david", "phone");
			c2.AddParty(party);
			s.Save(c2);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().Add(Restrictions.IdEq(c.Id)).UniqueResult<Contract>();
			c2 = s.CreateCriteria<Contract>().Add(Restrictions.IdEq(c2.Id)).UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c));
				}
				Assert.That(c2.Parties.Count, Is.EqualTo(0));
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				Assert.That(c2.Parties.Count, Is.EqualTo(1));
				party = c2.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c2));
				}
			}
			s.Delete(c);
			s.Delete(c2);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public virtual void MoveOneToManyElementToExistingEntityCollection()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			c.AddParty(new Party("party"));
			Contract c2 = new Contract(null, "david", "phone");
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			s.Persist(c2);
			t.Commit();
			s.Close();
	
			AssertInsertCount(3);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().Add(Restrictions.IdEq(c.Id)).UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(1));
			Party party = c.Parties.First();
			Assert.That(party.Name, Is.EqualTo("party"));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.SameAs(c));
			}
			c.RemoveParty(party);
			c2 = s.CreateCriteria<Contract>().Add(Restrictions.IdEq(c2.Id)).UniqueResult<Contract>();
			c2.AddParty(party);
			t.Commit();
			s.Close();
	
			AssertInsertCount(0);
			AssertUpdateCount(isContractVersioned ? 2 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().Add(Restrictions.IdEq(c.Id)).UniqueResult<Contract>();
			c2 = s.CreateCriteria<Contract>().Add(Restrictions.IdEq(c2.Id)).UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c));
				}
				Assert.That(c2.Parties.Count, Is.EqualTo(0));
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				Assert.That(c2.Parties.Count, Is.EqualTo(1));
				party = c2.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.SameAs(c2));
				}
			}
			s.Delete(c);
			s.Delete(c2);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public virtual void RemoveOneToManyElementUsingUpdate()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
			c.AddParty(party);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.RemoveParty(party);
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.Null);
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			s.Update(party);
			t.Commit();
			s.Close();
	
			if (CheckUpdateCountsAfterRemovingElementWithoutDelete())
			{
				AssertUpdateCount(isContractVersioned && !isContractPartiesInverse ? 1 : 0);
			}
			AssertDeleteCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				Assert.That(party.Contract, Is.SameAs(c));
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				party = s.CreateCriteria<Party>().UniqueResult<Party>();
				if (isContractPartiesBidirectional) {
					Assert.That(party.Contract, Is.Null);
				}
				s.Delete(party);
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void RemoveOneToManyElementUsingMerge()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
			c.AddParty(party);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.RemoveParty(party);
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.Null);
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = (Contract)s.Merge(c);
			party = (Party)s.Merge(party);
			t.Commit();
			s.Close();
	
			if (CheckUpdateCountsAfterRemovingElementWithoutDelete())
			{
				AssertUpdateCount(isContractVersioned && !isContractPartiesInverse ? 1 : 0);
			}
			AssertDeleteCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			if (isContractPartiesInverse)
			{
				Assert.That(c.Parties.Count, Is.EqualTo(1));
				party = c.Parties.First();
				Assert.That(party.Name, Is.EqualTo("party"));
				Assert.That(party.Contract, Is.SameAs(c));
			}
			else
			{
				Assert.That(c.Parties.Count, Is.EqualTo(0));
				party = s.CreateCriteria<Party>().UniqueResult<Party>();
				if (isContractPartiesBidirectional)
				{
					Assert.That(party.Contract, Is.Null);
				}
				s.Delete(party);
			}
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(2);
		}
	
		[Test]
		public virtual void DeleteOneToManyElement()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
			c.AddParty(party);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			c.RemoveParty(party);
			s.Delete(party);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			party = s.CreateCriteria<Party>().UniqueResult<Party>();
			Assert.That(party, Is.Null);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public virtual void RemoveOneToManyElementByDelete()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			Party party = new Party("party");
			c.AddParty(party);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.RemoveParty(party);
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			if (isContractPartiesBidirectional)
			{
				Assert.That(party.Contract, Is.Null);
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			s.Delete(party);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Parties.Count, Is.EqualTo(0));
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public virtual void RemoveOneToManyOrphanUsingUpdate()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			ContractVariation cv = new ContractVariation(c);
			cv.Text = "cv1";
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.Variations.Remove(cv);
			cv.Contract = null;
			Assert.That(c.Variations.Count, Is.EqualTo(0));
			
			if (isContractVariationsBidirectional)
			{
				Assert.That(cv.Contract, Is.Null);
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Variations.Count, Is.EqualTo(0));
			cv = s.CreateCriteria<ContractVariation>().UniqueResult<ContractVariation>();
			Assert.That(cv, Is.Null);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public virtual void RemoveOneToManyOrphanUsingMerge()
		{
			Contract c = new Contract(null, "gail", "phone");
			ContractVariation cv = new ContractVariation(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			c.Variations.Remove(cv);
			cv.Contract = null;
			Assert.That(c.Variations.Count, Is.EqualTo(0));
			if (isContractVariationsBidirectional)
			{
				Assert.That(cv.Contract, Is.Null);
			}
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = (Contract)s.Merge(c);
			cv = (ContractVariation)s.Merge(cv);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Variations.Count, Is.EqualTo(0));
			cv = s.CreateCriteria<ContractVariation>().UniqueResult<ContractVariation>();
			Assert.That(cv, Is.Null);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public virtual void DeleteOneToManyOrphan()
		{
			ClearCounts();
	
			Contract c = new Contract(null, "gail", "phone");
			ContractVariation cv = new ContractVariation(c);
	
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(c);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.Update(c);
			c.Variations.Remove(cv);
			cv.Contract = null;
			Assert.That(c.Variations.Count, Is.EqualTo(0));
			s.Delete(cv);
			t.Commit();
			s.Close();
	
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			AssertDeleteCount(1);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			Assert.That(c.Variations.Count, Is.EqualTo(0));
			cv = s.CreateCriteria<ContractVariation>().UniqueResult<ContractVariation>();
			Assert.That(cv, Is.Null);
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<ContractVariation>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(1);
		}
	
		[Test]
		public virtual void OneToManyCollectionOptimisticLockingWithMerge()
		{
			ClearCounts();
	
			Contract cOrig = new Contract(null, "gail", "phone");
			Party partyOrig = new Party("party");
			cOrig.AddParty(partyOrig);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(cOrig);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Contract c = s.Get<Contract>(cOrig.Id);
			Party newParty = new Party("new party");
			c.AddParty(newParty);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cOrig.RemoveParty(partyOrig);
			try
			{
				s.Merge(cOrig);
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
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
	
			AssertUpdateCount(0);
			AssertDeleteCount(3);
		}
	
		[Test]
		public virtual void OneToManyCollectionOptimisticLockingWithUpdate()
		{
			ClearCounts();
	
			Contract cOrig = new Contract(null, "gail", "phone");
			Party partyOrig = new Party("party");
			cOrig.AddParty(partyOrig);
			ISession s = OpenSession();
			ITransaction t = s.BeginTransaction();
			s.Persist(cOrig);
			t.Commit();
			s.Close();
	
			AssertInsertCount(2);
			AssertUpdateCount(0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Contract c = s.Get<Contract>(cOrig.Id);
			Party newParty = new Party("new party");
			c.AddParty(newParty);
			t.Commit();
			s.Close();
	
			AssertInsertCount(1);
			AssertUpdateCount(isContractVersioned ? 1 : 0);
			ClearCounts();
	
			s = OpenSession();
			t = s.BeginTransaction();
			cOrig.RemoveParty(partyOrig);
			s.Update(cOrig);
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
			c = s.CreateCriteria<Contract>().UniqueResult<Contract>();
			s.CreateQuery("delete from Party").ExecuteUpdate();
			s.Delete(c);
			Assert.That(s.CreateCriteria<Contract>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			Assert.That(s.CreateCriteria<Party>().SetProjection(Projections.RowCountInt64()).UniqueResult<long>(), Is.EqualTo(0L));
			t.Commit();
			s.Close();
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
