using System.Reflection;
using NHibernate.Engine;
using NHibernate.Impl;
using NUnit.Framework;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.NH1741
{
	[TestFixture]
	public class Fixture: BugTestCase
	{
		private const string QueryName = "NH1741_All";

		public class DetachedNamedQueryCrack : DetachedNamedQuery
		{
			private readonly FieldInfo fiCacheable = typeof(AbstractQueryImpl).GetField("cacheable", ReflectHelper.AnyVisibilityInstance);
			private readonly FieldInfo fiCacheRegion = typeof(AbstractQueryImpl).GetField("cacheRegion", ReflectHelper.AnyVisibilityInstance);
			private readonly FieldInfo fiCacheMode = typeof(AbstractQueryImpl).GetField("cacheMode", ReflectHelper.AnyVisibilityInstance);
			private readonly FieldInfo fiReadOnly = typeof(AbstractQueryImpl).GetField("readOnly", ReflectHelper.AnyVisibilityInstance);
			private readonly FieldInfo fiSelection = typeof(AbstractQueryImpl).GetField("selection", ReflectHelper.AnyVisibilityInstance);
			private readonly FieldInfo fiComment = typeof(AbstractQueryImpl).GetField("comment", ReflectHelper.AnyVisibilityInstance);
			private readonly FieldInfo fiFlushMode = typeof(AbstractQueryImpl).GetField("flushMode", ReflectHelper.AnyVisibilityInstance);

			private QueryImpl queryExecutable;
			public DetachedNamedQueryCrack(string queryName) : base(queryName) { }
			public override IQuery GetExecutableQuery(ISession session)
			{
				var result = base.GetExecutableQuery(session);
				queryExecutable = (QueryImpl)result;
				return result;
			}

			public bool Cacheable
			{
				get
				{
					return (bool)fiCacheable.GetValue(queryExecutable);
				}
			}

			public string CacheRegion
			{
				get
				{
					return (string)fiCacheRegion.GetValue(queryExecutable);
				}
			}

			public int Timeout
			{
				get
				{
					return ((RowSelection)fiSelection.GetValue(queryExecutable)).Timeout;
				}
			}

			public int FetchSize
			{
				get
				{
					return ((RowSelection)fiSelection.GetValue(queryExecutable)).FetchSize;
				}
			}

			public CacheMode? CacheMode
			{
				get
				{
					return (CacheMode?)fiCacheMode.GetValue(queryExecutable);
				}
			}

			public bool ReadOnly
			{
				get
				{
					return (bool)fiReadOnly.GetValue(queryExecutable);
				}
			}

			public string Comment
			{
				get
				{
					return (string)fiComment.GetValue(queryExecutable);
				}
			}

			public FlushMode FlushMode
			{
				get
				{
					return (FlushMode)fiFlushMode.GetValue(queryExecutable);
				}
			}
		}

		[Test]
		[Description("DetachedNamedQuery should read all mapped parameters when not explicitly set.")]
		public void Bug()
		{
			var dq = new DetachedNamedQueryCrack(QueryName);
			ISession s = Sfi.OpenSession();
			dq.GetExecutableQuery(s);
			s.Close();

			Assert.That(dq.Cacheable);
			Assert.That(dq.CacheRegion, Is.EqualTo("region"));
			Assert.That(dq.ReadOnly);
			Assert.That(dq.Timeout, Is.EqualTo(10));
			Assert.That(dq.CacheMode, Is.EqualTo(CacheMode.Normal));
			Assert.That(dq.FetchSize, Is.EqualTo(11));
			Assert.That(dq.Comment, Is.EqualTo("the comment"));
			Assert.That(dq.FlushMode, Is.EqualTo(FlushMode.Auto));
		}

		[Test]
		[Description("DetachedNamedQuery should override all mapped parameters when explicitly set.")]
		public void Override()
		{
			var dq = new DetachedNamedQueryCrack(QueryName);
			dq.SetCacheable(false).SetCacheRegion("another region").SetReadOnly(false).SetTimeout(20).SetCacheMode(
				CacheMode.Refresh).SetFetchSize(22).SetComment("another comment").SetFlushMode(FlushMode.Commit);
			ISession s = Sfi.OpenSession();
			dq.GetExecutableQuery(s);
			s.Close();

			Assert.That(!dq.Cacheable);
			Assert.That(dq.CacheRegion, Is.EqualTo("another region"));
			Assert.That(!dq.ReadOnly);
			Assert.That(dq.Timeout, Is.EqualTo(20));
			Assert.That(dq.CacheMode, Is.EqualTo(CacheMode.Refresh));
			Assert.That(dq.FetchSize, Is.EqualTo(22));
			Assert.That(dq.Comment, Is.EqualTo("another comment"));
			Assert.That(dq.FlushMode, Is.EqualTo(FlushMode.Commit));
		}
	}
}