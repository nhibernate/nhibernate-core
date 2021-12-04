﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NUnit.Framework;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.GH1962
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		[Test]
		public async Task LinqShouldBeValidAsync()
		{
			using (var session = OpenSession())
			{
				var result =
					await (session
						.Query<Product>()
						.CountAsync(p => p.OrderDetails.Any(od => od.Order.OrderDetails[0] == od)));
				Assert.That(result, Is.EqualTo(0));
			}
		}
	}
}
