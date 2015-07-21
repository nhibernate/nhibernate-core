using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1757
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		[Test]
		public void MayBeABug()
		{
			using (ISession s = OpenSession())
			{
				var query = s.CreateSQLQuery("SELECT SimpleEntity.*, 123 as field_not_in_entitytype FROM SimpleEntity")
					.AddEntity(typeof(SimpleEntity)) 
					.AddScalar("field_not_in_entitytype", NHibernateUtil.Int64); 
				IList<Object[]> result = query.List<Object[]>(); 
			}
		}
	}
}