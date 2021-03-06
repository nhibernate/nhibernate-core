﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Test.NHSpecificTest;
using NUnit.Framework;

namespace NHibernate.Test.OneToOneType
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Details").ExecuteUpdate();
				s.CreateQuery("delete from Owner").ExecuteUpdate();

				tx.Commit();
			}
		}

		[Test]
		public async Task OneToOnePersistedOnOwnerUpdateAsync()
		{
			object ownerId;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var owner = new Owner
				{
					Name = "Owner",
				};

				ownerId = await (s.SaveAsync(owner));

				await (tx.CommitAsync());
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Owner owner = await (s.LoadAsync<Owner>(ownerId));

				owner.Details = new Details
				{
					Data = "Owner Details"
				};

				await (tx.CommitAsync());
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				Owner owner = await (s.GetAsync<Owner>(ownerId));

				Assert.NotNull(owner.Details);
			}
		}

		[Test]
		public async Task OneToOnePersistedOnOwnerUpdateForSessionUpdateAsync()
		{
			Owner owner;

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				owner = new Owner
				{
					Name = "Owner",
				};

				await (s.SaveAsync(owner));
				await (tx.CommitAsync());
			}

			using (var s = Sfi.OpenSession())
			{
				owner = await (s.GetAsync<Owner>(owner.Id));
			}

			using (var s = Sfi.OpenSession())
			using (var tx = s.BeginTransaction())
			{
				await (s.SaveOrUpdateAsync(owner));
				owner.Details = new Details
				{
					Data = "Owner Details"
				};

				await (tx.CommitAsync());
			}

			using (var s = Sfi.OpenSession())
			{
				owner = await (s.GetAsync<Owner>(owner.Id));

				Assert.IsNotNull(owner.Details);
			}
		}
		
		[Test]
		public async Task CanInsertByStatelessSessionAsync()
		{
			object id;

			using (var s = Sfi.OpenStatelessSession())
			using (var tx = s.BeginTransaction())
			{
				var details = new Details
				{
					Owner = new Owner
					{
						Name = "Owner"
					},
					Data = "Owner Details"
				};

				id = await (s.InsertAsync(details));

				await (tx.CommitAsync());
			}

			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var owner = await (s.GetAsync<Owner>(id));

				Assert.That(owner.Details, Is.Not.Null);
			}
		}
	}
}
