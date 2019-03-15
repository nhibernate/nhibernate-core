﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH479
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		[Test]
		public async Task MergeTestAsync()
		{
			Main main = new Main();
			Aggregate aggregate = new Aggregate();

			main.Aggregate = aggregate;
			aggregate.Main = main;

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.SaveAsync(main));
				await (s.SaveAsync(aggregate));
				await (t.CommitAsync());
			}

			using (ISession s = OpenSession())
			{
				using (ITransaction t = s.BeginTransaction())
				{
					await (s.MergeAsync(main));
					await (s.MergeAsync(aggregate));
					await (t.CommitAsync());
				}

				using (ITransaction t = s.BeginTransaction())
				{
					await (s.DeleteAsync("from Aggregate"));
					await (s.DeleteAsync("from Main"));
					await (t.CommitAsync());
				}
			}
		}
	}
}
