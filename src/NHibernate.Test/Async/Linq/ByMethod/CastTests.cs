﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Exceptions;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq.ByMethod
{
	using System.Threading.Tasks;
	[TestFixture]
	public class CastTestsAsync : LinqTestCase
	{
		[Test]
		public async Task CastCountAsync()
		{
			Assert.That(await (session.Query<Cat>()
							   .Cast<Animal>()
							   .CountAsync()), Is.EqualTo(1));
		}

		[Test]
		public async Task CastWithWhereAsync()
		{
			var pregnatMammal = await ((from a
									in session.Query<Animal>().Cast<Cat>()
								  where a.Pregnant
								  select a).FirstOrDefaultAsync());
			Assert.That(pregnatMammal, Is.Not.Null);
		}

		[Test]
		public void CastDowncastAsync()
		{
			var query = session.Query<Mammal>().Cast<Dog>();
			List<Dog> list;
			// the list contains at least one Cat then should Throws
			// Do not use bare Throws.Exception due to https://github.com/nunit/nunit/issues/1899
			Assert.That(async () => list = await (query.ToListAsync()), Throws.InstanceOf<GenericADOException>());
		}

		[Test]
		public void OrderByAfterCastAsync()
		{
			// NH-2657
			var query = session.Query<Dog>().Cast<Animal>().OrderBy(a=> a.BodyWeight);
			Assert.That(() => query.ToListAsync(), Throws.Nothing);
		}

		[Test, Ignore("Not fixed yet. The method OfType does not work as expected.")]
		public void CastDowncastUsingOfTypeAsync()
		{
			var query = session.Query<Animal>().OfType<Mammal>().Cast<Dog>();
			// the list contains at least one Cat then should Throws
			// Do not use bare Throws.Exception due to https://github.com/nunit/nunit/issues/1899
			Assert.That(() => query.ToListAsync(), Throws.InstanceOf<Exception>());
		}
	}
}
