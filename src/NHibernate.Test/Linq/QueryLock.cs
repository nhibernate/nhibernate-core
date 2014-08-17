using System.Linq;
using NHibernate.AdoNet;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	public class QueryLock : LinqTestCase
	{

		[Test]
		public void CanSetLockLinqQueries()
		{
			var result = (from e in db.Customers
						  where e.CompanyName == "Corp"
						  select e).SetLockMode(LockMode.Upgrade).ToList();

		}


		[Test]
		public void CanSetLockOnLinqPagingQuery()
		{
			var result = (from e in db.Customers
						  where e.CompanyName == "Corp"
						  select e).Skip(5).Take(5).SetLockMode(LockMode.Upgrade).ToList();
		}


		[Test]
		public void CanLockBeforeSkipOnLinqOrderedPageQuery()
		{
			var result = (from e in db.Customers
						  orderby e.CompanyName
						  select e)
				.SetLockMode(LockMode.Upgrade).Skip(5).Take(5).ToList();


		}


	}

}

