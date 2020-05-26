﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1948
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		[Test]
		public async Task CanUseDecimalScaleZeroAsync()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person person =
					new Person()
					{
						Age = 50,
						ShoeSize = 10,
						FavouriteNumbers =
							new List<decimal>()
							{
								20,
								30,
								40,
							},
					};

				await (s.SaveAsync(person));
				await (s.FlushAsync());
				await (tx.RollbackAsync());
			}
		}
	}
}
