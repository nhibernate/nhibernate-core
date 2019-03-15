﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Multi;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2195
{
	[TestFixture]
	public class SQLiteMultiCriteriaTest : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = this.OpenSession())
			{
				DomainClass entity = new DomainClass();
				entity.Id = 1;
				entity.StringData = "John Doe";
				entity.IntData = 1;
				session.Save(entity);

				entity = new DomainClass();
				entity.Id = 2;
				entity.StringData = "Jane Doe";
				entity.IntData = 2;
				session.Save(entity);
				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = this.OpenSession())
			{
				string hql = "from System.Object";
				session.Delete(hql);
				session.Flush();
			}
		}

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect as SQLiteDialect != null;
		}

		[Test]
		public void SingleCriteriaQueriesWithIntsShouldExecuteCorrectly()
		{
			// Test querying IntData
			using (ISession session = this.OpenSession())
			{
				ICriteria criteriaWithPagination = session.CreateCriteria<DomainClass>();
				criteriaWithPagination.Add(Expression.Le("IntData",2));
				ICriteria criteriaWithRowCount = CriteriaTransformer.Clone(criteriaWithPagination);
				criteriaWithPagination.SetFirstResult(0).SetMaxResults(1);
				criteriaWithRowCount.SetProjection(Projections.RowCountInt64());

				IList<DomainClass> list = criteriaWithPagination.List<DomainClass>();

				Assert.AreEqual(2, criteriaWithRowCount.UniqueResult<long>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void SingleCriteriaQueriesWithStringsShouldExecuteCorrectly()
		{
			// Test querying StringData
			using (ISession session = this.OpenSession())
			{
				ICriteria criteriaWithPagination = session.CreateCriteria<DomainClass>();
				criteriaWithPagination.Add(Expression.Like("StringData", "%Doe%"));
				ICriteria criteriaWithRowCount = CriteriaTransformer.Clone(criteriaWithPagination);
				criteriaWithPagination.SetFirstResult(0).SetMaxResults(1);
				criteriaWithRowCount.SetProjection(Projections.RowCountInt64());

				IList<DomainClass> list = criteriaWithPagination.List<DomainClass>();

				Assert.AreEqual(2, criteriaWithRowCount.UniqueResult<long>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test, Obsolete]
		public void MultiCriteriaQueriesWithIntsShouldExecuteCorrectly()
		{
			var driver = Sfi.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);

			// Test querying IntData
			using (ISession session = this.OpenSession())
			{
				ICriteria criteriaWithPagination = session.CreateCriteria<DomainClass>();
				criteriaWithPagination.Add(Expression.Le("IntData", 2));
				ICriteria criteriaWithRowCount = CriteriaTransformer.Clone(criteriaWithPagination);
				criteriaWithPagination.SetFirstResult(0).SetMaxResults(1);
				criteriaWithRowCount.SetProjection(Projections.RowCountInt64());

				IMultiCriteria multiCriteria = session.CreateMultiCriteria();
				multiCriteria.Add(criteriaWithPagination);
				multiCriteria.Add(criteriaWithRowCount);

				IList results = multiCriteria.List();
				long numResults = (long)((IList)results[1])[0];
				IList list = (IList)results[0];

				Assert.AreEqual(2, criteriaWithRowCount.UniqueResult<long>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test, Obsolete]
		public void MultiCriteriaQueriesWithStringsShouldExecuteCorrectly()
		{
			var driver = Sfi.ConnectionProvider.Driver;
			if (!driver.SupportsMultipleQueries)
				Assert.Ignore("Driver {0} does not support multi-queries", driver.GetType().FullName);

			// Test querying StringData
			using (ISession session = this.OpenSession())
			{
				ICriteria criteriaWithPagination = session.CreateCriteria<DomainClass>();
				criteriaWithPagination.Add(Expression.Like("StringData", "%Doe%"));
				ICriteria criteriaWithRowCount = CriteriaTransformer.Clone(criteriaWithPagination);
				criteriaWithPagination.SetFirstResult(0).SetMaxResults(1);
				criteriaWithRowCount.SetProjection(Projections.RowCountInt64());

				IMultiCriteria multiCriteria = session.CreateMultiCriteria();
				multiCriteria.Add(criteriaWithPagination);
				multiCriteria.Add(criteriaWithRowCount);

				IList results = multiCriteria.List();

				long numResults = (long)((IList)results[1])[0];
				IList list = (IList)results[0];

				Assert.AreEqual(2, criteriaWithRowCount.UniqueResult<long>());
				Assert.AreEqual(1, list.Count);
			}
		}

		[Test]
		public void QueryBatchWithIntsShouldExecuteCorrectly()
		{
			// Test querying IntData
			using (var session = OpenSession())
			{
				var criteriaWithPagination = session.CreateCriteria<DomainClass>();
				criteriaWithPagination.Add(Restrictions.Le("IntData", 2));
				var criteriaWithRowCount = CriteriaTransformer.Clone(criteriaWithPagination);
				criteriaWithPagination.SetFirstResult(0).SetMaxResults(1);
				criteriaWithRowCount.SetProjection(Projections.RowCountInt64());

				var multi = session.CreateQueryBatch();
				multi.Add<DomainClass>(criteriaWithPagination);
				multi.Add<long>(criteriaWithRowCount);

				var numResults = multi.GetResult<long>(1).Single();
				var list = multi.GetResult<DomainClass>(0);

				Assert.That(numResults, Is.EqualTo(2));
				Assert.That(list.Count, Is.EqualTo(1));
				Assert.That(criteriaWithRowCount.UniqueResult<long>(), Is.EqualTo(2));
			}
		}

		[Test]
		public void QueryBatchWithStringsShouldExecuteCorrectly()
		{
			// Test querying StringData
			using (var session = OpenSession())
			{
				var criteriaWithPagination = session.CreateCriteria<DomainClass>();
				criteriaWithPagination.Add(Restrictions.Like("StringData", "%Doe%"));
				var criteriaWithRowCount = CriteriaTransformer.Clone(criteriaWithPagination);
				criteriaWithPagination.SetFirstResult(0).SetMaxResults(1);
				criteriaWithRowCount.SetProjection(Projections.RowCountInt64());

				var multi = session.CreateQueryBatch();
				multi.Add<DomainClass>(criteriaWithPagination);
				multi.Add<long>(criteriaWithRowCount);

				var numResults = multi.GetResult<long>(1).Single();
				var list = multi.GetResult<DomainClass>(0);

				Assert.That(numResults, Is.EqualTo(2));
				Assert.That(list.Count, Is.EqualTo(1));
				Assert.That(criteriaWithRowCount.UniqueResult<long>(), Is.EqualTo(2));
			}
		}
	}
}
