﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1868
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					cat = new Category {ValidUntil = DateTime.Now};
					session.Save(cat);

					package = new Package {ValidUntil = DateTime.Now};
					session.Save(package);

					tx.Commit();
				}
			}
		}

		private Category cat;
		private Package package;

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Category");
					session.Delete("from Package");
					tx.Commit();
				}
			}
			base.OnTearDown();
		}

		public async Task ExecuteQueryAsync(Action<ISession> sessionModifier, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					sessionModifier(session);
					await (session.RefreshAsync(cat, cancellationToken));
					await (session.RefreshAsync(package, cancellationToken));

					await (session.CreateQuery(
						@"
                    select 
                        inv
                    from 
                        Invoice inv
                        , Package p
                    where
                        p = :package
                        and inv.Category = :cat
                        and inv.ValidUntil > :now
                        and inv.Package = :package 
                    ")
						.SetEntity("cat", cat).SetEntity("package", package).SetDateTime("now", DateTime.Now).UniqueResultAsync<Invoice>(cancellationToken));

					await (tx.CommitAsync(cancellationToken));
				}
			}
		}
	}
}
