using System;
using System.Diagnostics;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3141
{
	[TestFixture]
	public class ProxyIdPerformanceTest : BugTestCase
	{
		private int id;

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					id = (int)s.Save(new Entity());
					tx.Commit();
				}
			}
		}

		[Test, Explicit("No logical test - just to compare before/after fix")]
		public void ShouldUseIdDirectlyFromProxy()
		{
			var proxyEntity = createInitializedProxy();

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

		private Entity createInitializedProxy()
		{
			using (var s = OpenSession())
			{
				using (s.BeginTransaction())
				{
					var proxyEntity = s.Load<Entity>(id);
					NHibernateUtil.Initialize(proxyEntity);
					return proxyEntity;
				}
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			{
				using (var tx = s.BeginTransaction())
				{
					s.CreateQuery("delete from Entity e").ExecuteUpdate();
					tx.Commit();
				}
			}
		}
	}
}