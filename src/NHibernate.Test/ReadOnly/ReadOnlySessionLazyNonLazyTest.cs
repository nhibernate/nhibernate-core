using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.SqlCommand;
using NHibernate.Transform;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.ReadOnly
{
	/* This Hibernate port excludes the 'lazy="no-proxy" mappings because they fail with the
	 * NHibernate.ByteCode.LinFu.ProxyFactory - it does not provide an implementation
	 * of GetFieldInterceptionProxy(). LinFu is the default bytecode provider used for CI, so
	 * these mappings cannot be included.
	 *
	 * Model:
	 *
	 * Container
	 * |-- N : 1 -- noProxyOwner (property-ref="name" lazy="no-proxy" cascade="all")
	 * |-- N : 1 -- proxyOwner (property-ref="name" lazy="proxy" cascade="all")
	 * |-- N : 1 -- nonLazyOwner (property-ref="name" lazy="false" cascade="all")
	 * |-- N : 1 -- noProxyInfo" (lazy="no-proxy" cascade="all")
	 * |-- N : 1 -- proxyInfo (lazy="proxy" cascade="all"
	 * |-- N : 1 -- nonLazyInfo" (lazy="false" cascade="all")
	 * |
	 * |-- 1 : N -- lazyDataPoints" (lazy="true" inverse="false" cascade="all")
	 * |-- 1 : N -- nonLazySelectDataPoints" (lazy="false" inverse="false" cascade="all" fetch="select")
	 * |-- 1 : N -- nonLazyJoinDataPoints" (lazy="false" inverse="false" cascade="all" fetch="join")
	 *
	 * Note: the following many-to-one properties use a property-ref so they are
	 * initialized, regardless of how the lazy attribute is mapped:
	 * noProxyOwner, proxyOwner, nonLazyOwner
	 *
	 * @author Gail Badner
	 */
	
	[TestFixture]
	public class ReadOnlySessionLazyNonLazyTest : AbstractReadOnlyTest
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "ReadOnly.DataPoint.hbm.xml" }; }
		}
		
		[Test]
		public void ExistingModifiableAfterSetSessionReadOnly()
		{
			Container cOrig = CreateContainer();

			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
	
			t = s.BeginTransaction();
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Container c = s.Load<Container>(cOrig.Id);
			Assert.That(cOrig, Is.SameAs(c));
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.SameAs(c));
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.Refresh(cOrig);
			Assert.That(cOrig, Is.SameAs(c));
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			
			// NH-specific: The following line is required to evict DataPoint(Id=1) from the Container.LazyDataPoint collection.
			// This behaviour would seem to be necessary 'by design', as a comment in EvictCascadingAction states, "evicts don't
			// cascade to uninitialized collections".
			// If LazyDataPoint(Id=1) is not evicted, it has a status of Loaded, not ReadOnly, and causes the test to fail further
			// down.
			// Another way to get this test to pass is s.Clear().
			NHibernateUtil.Initialize(cOrig.LazyDataPoints);
			
			s.Evict(cOrig);

			c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
		
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});
			
			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			expectedReadOnlyObjects.Add(c.LazyDataPoints.First());
			
			// The following check fails if the NH-specific change (above) is not made. More specifically it fails
			// when asserting that the c.LazyDataPoints.First() is ReadOnly
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			
			t.Commit();
			s.Close();
			
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void ExistingReadOnlyAfterSetSessionModifiable()
		{
			Container cOrig = CreateContainer();

			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
			
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
			
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = false;
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			//expectedReadOnlyObjects.Add(c.LazyDataPoints.First());
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void ExistingReadOnlyAfterSetSessionModifiableExisting()
		{
	
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
			DataPoint lazyDataPointOrig = cOrig.LazyDataPoints.First();
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
			
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c, c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = false;
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			DataPoint lazyDataPoint = s.Get<DataPoint>(lazyDataPointOrig.Id);
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			Assert.That(lazyDataPoint, Is.SameAs(c.LazyDataPoints.First()));
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
	
		}
	
		[Test]
		public void ExistingReadOnlyAfterSetSessionModifiableExistingEntityReadOnly()
		{
	
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
			DataPoint lazyDataPointOrig = cOrig.LazyDataPoints.First();
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));

			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = false;
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			DataPoint lazyDataPoint = s.Get<DataPoint>(lazyDataPointOrig.Id);
			s.DefaultReadOnly = false;
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			Assert.That(lazyDataPoint, Is.SameAs(c.LazyDataPoints.First()));
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			expectedReadOnlyObjects.Add(lazyDataPoint);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void ExistingReadOnlyAfterSetSessionModifiableProxyExisting()
		{
	
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
			DataPoint lazyDataPointOrig = cOrig.LazyDataPoints.First();
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
			
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = false;
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			DataPoint lazyDataPoint = s.Load<DataPoint>(lazyDataPointOrig.Id);
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			Assert.That(lazyDataPoint, Is.SameAs(c.LazyDataPoints.First()));
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
	
		}
	
		[Test]
		public void ExistingReadOnlyAfterSetSessionModifiableExistingProxyReadOnly()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
			DataPoint lazyDataPointOrig = cOrig.LazyDataPoints.First();
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
			
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = false;
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			DataPoint lazyDataPoint = s.Load<DataPoint>(lazyDataPointOrig.Id);
			s.DefaultReadOnly = false;
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			Assert.That(lazyDataPoint, Is.SameAs(c.LazyDataPoints.First()));
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			expectedReadOnlyObjects.Add(lazyDataPoint);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultModifiableWithReadOnlyQueryForEntity()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Assert.That(s.DefaultReadOnly, Is.False);
			Container c = s.CreateQuery("from Container where id=" + cOrig.Id).SetReadOnly(true).UniqueResult<Container>();
			
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			//expectedReadOnlyObjects.Add(c.LazyDataPoints.First());
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultReadOnlyWithModifiableQueryForEntity()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			Container c = s.CreateQuery("from Container where id=" + cOrig.Id).SetReadOnly(false).UniqueResult<Container>();
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects = new HashSet<object>();
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			expectedReadOnlyObjects.Add(c.LazyDataPoints.First());
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultReadOnlyWithQueryForEntity()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			Container c = s.CreateQuery("from Container where id=" + cOrig.Id).UniqueResult<Container>();

			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			expectedReadOnlyObjects.Add(c.LazyDataPoints.First());
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultModifiableWithQueryForEntity()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Assert.That(s.DefaultReadOnly, Is.False);
			Container c = s.CreateQuery("from Container where id=" + cOrig.Id).UniqueResult<Container>();
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects = new HashSet<object>();
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
//			Assert.That(NHibernateUtil.IsInitialized(c.NoProxyInfo), Is.False);
//			NHibernateUtil.Initialize(c.NoProxyInfo);
//			expectedInitializedObjects.Add(c.NoProxyInfo);
//			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.ProxyInfo), Is.False);
			NHibernateUtil.Initialize(c.ProxyInfo);
			expectedInitializedObjects.Add(c.ProxyInfo);
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			Assert.That(NHibernateUtil.IsInitialized(c.LazyDataPoints), Is.False);
			NHibernateUtil.Initialize(c.LazyDataPoints);
			expectedInitializedObjects.Add(c.LazyDataPoints.First());
			//expectedReadOnlyObjects.Add(c.LazyDataPoints.First());
			CheckContainer(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultModifiableWithReadOnlyQueryForCollectionEntities()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Assert.That(s.DefaultReadOnly, Is.False);
			DataPoint dp = s.CreateQuery("select c.LazyDataPoints from Container c join c.LazyDataPoints where c.Id=" + cOrig.Id)
				.SetReadOnly(true)
				.UniqueResult<DataPoint>();
			Assert.That(s.IsReadOnly(dp), Is.True);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultReadOnlyWithModifiableFilterCollectionEntities()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));

			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			IList list = s.CreateFilter(c.LazyDataPoints, "").SetMaxResults(1).SetReadOnly(false).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.False);
			list = s.CreateFilter(c.NonLazyJoinDataPoints, "").SetMaxResults(1).SetReadOnly(false).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.True);
			list = s.CreateFilter(c.NonLazySelectDataPoints, "").SetMaxResults(1).SetReadOnly(false).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.True);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultModifiableWithReadOnlyFilterCollectionEntities()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Assert.That(s.DefaultReadOnly, Is.False);
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects = new HashSet<object>();
			IList list = s.CreateFilter(c.LazyDataPoints, "").SetMaxResults(1).SetReadOnly(true).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.True);
			list = s.CreateFilter(c.NonLazyJoinDataPoints, "").SetMaxResults(1).SetReadOnly(true).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.False);
			list = s.CreateFilter(c.NonLazySelectDataPoints, "").SetMaxResults(1).SetReadOnly(true).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.False);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultReadOnlyWithFilterCollectionEntities()
		{
			Container cOrig = CreateContainer();
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects =
					new HashSet<object>(
						new object[]
						{
							c,
							//c.NoProxyInfo,
							c.ProxyInfo,
							c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							//c.getLazyDataPoints(),
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			IList list = s.CreateFilter(c.LazyDataPoints, "").SetMaxResults(1).List();
			
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.True);
			list = s.CreateFilter(c.NonLazyJoinDataPoints, "").SetMaxResults(1).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.True);
			list = s.CreateFilter(c.NonLazySelectDataPoints, "").SetMaxResults(1).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.True);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
	
		[Test]
		public void DefaultModifiableWithFilterCollectionEntities()
		{
			Container cOrig = CreateContainer();
			
			ISet<object> expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							cOrig,
							//cOrig.NoProxyInfo,
							cOrig.ProxyInfo,
							cOrig.NonLazyInfo,
							//cOrig.NoProxyOwner,
							cOrig.ProxyOwner,
							cOrig.NonLazyOwner,
							cOrig.LazyDataPoints.First(),
							cOrig.NonLazyJoinDataPoints.First(),
							cOrig.NonLazySelectDataPoints.First()
						});

			ISet<object> expectedReadOnlyObjects = new HashSet<object>();
	
			ISession s = OpenSession();
			Assert.That(s.DefaultReadOnly, Is.False);
			ITransaction t = s.BeginTransaction();
			s.Save(cOrig);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			s.DefaultReadOnly = true;
			Assert.That(s.DefaultReadOnly, Is.True);
			CheckContainer(cOrig, expectedInitializedObjects, expectedReadOnlyObjects, s);
			t.Commit();
			s.Close();
	
			s = OpenSession();
			t = s.BeginTransaction();
			Assert.That(s.DefaultReadOnly, Is.False);
			Container c = s.Get<Container>(cOrig.Id);
			Assert.That(cOrig, Is.Not.SameAs(c));
			expectedInitializedObjects =
					new HashSet<object>(
						new object[]
						{
							c, c.NonLazyInfo,
							//c.NoProxyOwner,
							c.ProxyOwner,
							c.NonLazyOwner,
							c.NonLazyJoinDataPoints.First(),
							c.NonLazySelectDataPoints.First()
						});

			expectedReadOnlyObjects = new HashSet<object>();
			IList list = s.CreateFilter(c.LazyDataPoints, "" ).SetMaxResults(1).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.False);
			list = s.CreateFilter(c.NonLazyJoinDataPoints, "").SetMaxResults(1).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.False);
			list = s.CreateFilter(c.NonLazySelectDataPoints, "").SetMaxResults(1).List();
			Assert.That(list.Count, Is.EqualTo(1));
			Assert.That(s.IsReadOnly(list[0]), Is.False);
			t.Commit();
			s.Close();
			s = OpenSession();
			t = s.BeginTransaction();
			s.CreateQuery("delete from DataPoint").ExecuteUpdate();
			s.CreateQuery("delete from Container").ExecuteUpdate();
			s.CreateQuery("delete from Info").ExecuteUpdate();
			s.CreateQuery("delete from Owner").ExecuteUpdate();
			t.Commit();
			s.Close();
		}
			
		
		private Container CreateContainer()
		{
			Container c = new Container("container");
			
			//c.NoProxyInfo = new Info("no-proxy info");
			c.ProxyInfo = new Info("proxy info");
			c.NonLazyInfo = new Info("non-lazy info");
			//c.NoProxyOwner = new Owner("no-proxy owner");
			c.ProxyOwner = new Owner("proxy owner");
			c.NonLazyOwner = new Owner("non-lazy owner");
			
			c.LazyDataPoints.Add(new DataPoint(1M, 1M, "lazy data point"));
			c.NonLazyJoinDataPoints.Add(new DataPoint(2M, 2M, "non-lazy join data point"));
			c.NonLazySelectDataPoints.Add(new DataPoint(3M, 3M, "non-lazy select data point"));
			
			return c;
		}
	
		private void CheckContainer(Container c, ISet<object> expectedInitializedObjects, ISet<object> expectedReadOnlyObjects, ISession s)
		{
			CheckObject(c, expectedInitializedObjects, expectedReadOnlyObjects, s);
			
			if (!expectedInitializedObjects.Contains(c))
			{
				return;
			}
			
			//CheckObject(c.NoProxyInfo, expectedInitializedObjects, expectedReadOnlyObjects, s);
			CheckObject(c.ProxyInfo, expectedInitializedObjects, expectedReadOnlyObjects, s);
			CheckObject(c.NonLazyInfo, expectedInitializedObjects, expectedReadOnlyObjects, s );
			//CheckObject(c.NoProxyOwner, expectedInitializedObjects, expectedReadOnlyObjects, s );
			CheckObject(c.ProxyOwner, expectedInitializedObjects, expectedReadOnlyObjects, s );
			CheckObject(c.NonLazyOwner, expectedInitializedObjects, expectedReadOnlyObjects, s );
			
			if (NHibernateUtil.IsInitialized(c.LazyDataPoints))
			{
				foreach(DataPoint dp in c.LazyDataPoints)
					CheckObject(dp, expectedInitializedObjects, expectedReadOnlyObjects, s);

				foreach(DataPoint dp in c.NonLazyJoinDataPoints)
					CheckObject(dp, expectedInitializedObjects, expectedReadOnlyObjects, s);
				
				foreach(DataPoint dp in c.NonLazySelectDataPoints)
					CheckObject(dp, expectedInitializedObjects, expectedReadOnlyObjects, s);
			}
		}
	
		private void CheckObject(object entityOrProxy, ISet<object> expectedInitializedObjects, ISet<object> expectedReadOnlyObjects, ISession s)
		{
			bool isExpectedToBeInitialized = expectedInitializedObjects.Contains(entityOrProxy);
			bool isExpectedToBeReadOnly = expectedReadOnlyObjects.Contains(entityOrProxy);
			ISessionImplementor si = (ISessionImplementor)s;
			Assert.That(NHibernateUtil.IsInitialized(entityOrProxy), Is.EqualTo(isExpectedToBeInitialized));
			Assert.That(s.IsReadOnly(entityOrProxy), Is.EqualTo(isExpectedToBeReadOnly));
			if (NHibernateUtil.IsInitialized(entityOrProxy))
			{
				object entity = entityOrProxy is INHibernateProxy
				                 ?((INHibernateProxy)entityOrProxy).HibernateLazyInitializer.GetImplementation(si)
				                 : entityOrProxy;
				Assert.That(entity, Is.Not.Null);
				Assert.That(s.IsReadOnly(entity), Is.EqualTo(isExpectedToBeReadOnly));
			}
		}
	}
}
