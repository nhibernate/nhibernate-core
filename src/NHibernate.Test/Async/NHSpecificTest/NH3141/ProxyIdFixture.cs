﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3141
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class ProxyIdFixtureAsync : BugTestCase
	{
		private int id;

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return TestDialect.SupportsEmptyInsertsOrHasNonIdentityNativeGenerator;
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				id = (int) s.Save(new Entity());
				tx.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.CreateQuery("delete from Entity e").ExecuteUpdate();
				tx.Commit();
			}
		}

		[Test, Explicit("No logical test - just to compare before/after fix")]
		public async Task ShouldUseIdDirectlyFromProxyAsync()
		{
			var proxyEntity = await (CreateInitializedProxyAsync());

			const int loop = 1000000;
			var watch = new Stopwatch();
			watch.Start();
			const int dummyValue = 0;
			for (var i = 0; i < loop; i++)
			{
				dummyValue.CompareTo(proxyEntity.Id);
			}
			watch.Stop();

			//before fix: 2.2s
			//after fix: 0.8s
			Console.WriteLine(watch.Elapsed);
		}
		
		[Test]
		public async Task ShouldThrowExceptionIfIdChangedOnUnloadEntityAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var entity = await (s.LoadAsync<Entity>(id));
				entity.Id ++;
				Assert.ThrowsAsync<HibernateException>(() => tx.CommitAsync());
			}
		}

		[Test]
		public async Task ShouldThrowExceptionIfIdChangedOnLoadEntityAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var entity = await (s.LoadAsync<Entity>(id));
				await (NHibernateUtil.InitializeAsync(entity));
				entity.Id++;
				Assert.ThrowsAsync<HibernateException>(() => tx.CommitAsync());
			}
		}

		private async Task<Entity> CreateInitializedProxyAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var proxyEntity = await (s.LoadAsync<Entity>(id, cancellationToken));
				await (NHibernateUtil.InitializeAsync(proxyEntity, cancellationToken));
				return proxyEntity;
			}
		}
	}
}
