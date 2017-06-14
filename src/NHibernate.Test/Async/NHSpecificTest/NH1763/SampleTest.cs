﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1763
{
	using System.Threading.Tasks;
	[TestFixture]
	public class SampleTestAsync : BugTestCase
	{
		[Test]
		public async Task CanUseConditionalOnCompositeTypeAsync()
		{
			using (ISession session = OpenSession())
			{
				await (session.CreateCriteria<Customer>()
					.SetProjection(Projections.Conditional(Restrictions.IdEq(1),
					                                       Projections.Property("Name"),
					                                       Projections.Property("Name2")))
					.ListAsync());
			}
		}
	}
}