﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest.Query
{
	using System.Threading.Tasks;
	[TestFixture]
	public class SelfReferencingCollectionLoadTestAsync : TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] {"SqlTest.Query.Item.hbm.xml"}; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			// Hacky mapping causing the primary key to reference itself as a foreign key, which is not supported by
			// some databases. It fails when trying to insert data by considering the foreign key violated.
			return !(Dialect is MySQLDialect || Dialect is SapSQLAnywhere17Dialect);
		}

		[Test]
		public async Task LoadCollectionAsync()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					await (session.SaveAsync(new Item(1, 2)));
					await (session.SaveAsync(new Item(2, 3)));
					await (session.SaveAsync(new Item(3, 1)));

					await (tx.CommitAsync());
				}
			}

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var item1 = (Item) await (session.GetAsync(typeof (Item), 1));
					Assert.AreEqual(2, item1.AlternativeItems.Count);

					await (session.DeleteAsync("from Item"));

					await (tx.CommitAsync());
				}
			}
		}
	}
}
