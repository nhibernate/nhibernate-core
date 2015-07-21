using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3141
{
	[TestFixture]
	public class ProxyIdFixture : BugTestCase
	{
		private int id;

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
		public void ShouldUseIdDirectlyFromProxy()
		{
			var proxyEntity = CreateInitializedProxy();

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
		public void ShouldThrowExceptionIfIdChangedOnUnloadEntity()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var entity = s.Load<Entity>(id);
				entity.Id ++;
				Assert.Throws<HibernateException>(tx.Commit);
			}
		}

		[Test]
		public void ShouldThrowExceptionIfIdChangedOnLoadEntity()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var entity = s.Load<Entity>(id);
				NHibernateUtil.Initialize(entity);
				entity.Id++;
				Assert.Throws<HibernateException>(tx.Commit);
			}
		}

		private Entity CreateInitializedProxy()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var proxyEntity = s.Load<Entity>(id);
				NHibernateUtil.Initialize(proxyEntity);
				return proxyEntity;
			}
		}
	}
}
