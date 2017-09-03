using System;
using System.Linq;
using System.Reflection;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class ExpressionSessionLeakTest : LinqTestCase
	{
		[Test]
		public void SessionGetsCollected()
		{
			var reference = DoLinqInSeparateSession();

			GC.Collect();

			Assert.That(reference.IsAlive, Is.False);
		}

		private WeakReference DoLinqInSeparateSession()
		{
			// Using SessionFactory of another session dodge TestCase session tracking, which is needed for having session garbage collected
			// Otherwise DebugSessionFactory session tracking should be changed to use WeakReference too.
			using (var leakTest = session.SessionFactory.OpenSession())
			{
				// It appears linq expressions will (or might) contain a reference to the
				// IQueryable. At time of writing, linq expressions are held within NhLinqExpression,
				// which in turn will be held in the query plan cache. Since the IQueryable will
				// be an NhQueryable, which holds a reference to the SessionImpl instance,
				// we will be leaking session instances.

				var query = leakTest.Query<Customer>().FirstOrDefault(t => t.CustomerId != "");

				return new WeakReference(leakTest, false);
			}
		}

		static readonly PropertyInfo SessionProperty = typeof(DefaultQueryProvider).GetProperty(
			"Session",
			BindingFlags.NonPublic | BindingFlags.Instance);

		[Theory]
		public void SessionIsNotNullOrResurrected(bool? disposeSession)
		{
			// Must do in a separated method, local variables seem not collected otherwise.
			var provider = GetProviderFromSeparateSession(disposeSession);

			if (provider == null)
				Assert.Ignore("Another query provider than NHibernate default one is used");
			Assert.That(SessionProperty, Is.Not.Null, $"Session property on {nameof(DefaultQueryProvider)} is not found.");

			// Force collection of no more strongly referenced objects.
			// Do not wait for pending finalizers
			GC.Collect();

			try
			{
				var s = SessionProperty.GetValue(provider);
				Assert.Fail($"Getting provider Session property did not failed. Obtained {(s == null ? "null" : s.GetType().Name)}.");
			}
			catch (TargetInvocationException tie)
			{
				Assert.That(tie.InnerException, Is.TypeOf<InvalidOperationException>().And.Message.Contains("garbage coll"));
			}
		}

		[Theory]
		public void QueryFailsProperlyOnDereferencedSession(bool? disposeSession)
		{
			// Must do in a separated method, local variables seem not collected otherwise.
			var query = GetQueryFromSeparateSession(disposeSession);

			// Force collection of no more strongly referenced objects.
			// Do not wait for pending finalizers
			GC.Collect();

			Assert.That(() => query.FirstOrDefault(), Throws.InvalidOperationException.And.Message.Contains("garbage coll"));
		}

		IQueryable<Customer> GetQueryFromSeparateSession(bool? disposeSession)
		{
			// Using SessionFactory of another session dodge DebugSessionFactory session tracking, which is needed for having session garbage collected.
			// Otherwise DebugSessionFactory session tracking should be changed to use WeakReference too.
			var s = session.SessionFactory.OpenSession();
			try
			{
				return s.Query<Customer>();
			}
			finally
			{
				if (disposeSession == true)
					s.Dispose();
				else if (disposeSession == false)
					s.Close();
			}
		}

		DefaultQueryProvider GetProviderFromSeparateSession(bool? disposeSession)
		{
			var queryable = GetQueryFromSeparateSession(disposeSession);
			return queryable.Provider as DefaultQueryProvider;
		}
	}
}
