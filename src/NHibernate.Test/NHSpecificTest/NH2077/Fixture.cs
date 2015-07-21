using System;
using System.Collections.Generic;
using System.Data;
using System.Transactions;
using NHibernate.Dialect;
using NHibernate.Impl;
using NUnit.Framework;
using NHibernate.Criterion;

namespace NHibernate.Test.NHSpecificTest.NH2077
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		protected override bool AppliesTo(NHibernate.Dialect.Dialect dialect)
		{
			return dialect is MsSql2000Dialect;
		}
     
		[Test]
      	public void CanExecuteMultipleQueriesUsingNativeSQL()
		{
            using (var s = OpenSession())
            {
            	s.CreateSQLQuery(
					@"
DELETE FROM Person WHERE Id = :userId; 
UPDATE Person SET Id = :deletedUserId WHERE Id = :userId; 
DELETE FROM Person WHERE Id = :userId; 
")
 .SetParameter("userId",1)
 .SetParameter("deletedUserId", 1)
            		.ExecuteUpdate();
            }
		} 

	}
}
