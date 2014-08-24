using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Test.NHSpecificTest.NH901
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH901"; }
		}

		private new ISession OpenSession(IInterceptor interceptor)
		{
			lastOpenedSession = sessions.OpenSession(interceptor);
			return lastOpenedSession;
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public void EmptyValueTypeComponent()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = new Person("Jimmy Hendrix");
				s.Save(p);
				tx.Commit();
			}

			InterceptorStub interceptor = new InterceptorStub();
			using (ISession s = OpenSession(interceptor))
			using (ITransaction tx = s.BeginTransaction())
			{
				Person jimmy = s.Get<Person>("Jimmy Hendrix");
				interceptor.entityToCheck = jimmy;
				tx.Commit();
			}
			Assert.IsFalse(interceptor.entityWasDeemedDirty);

			InterceptorStub interceptor2 = new InterceptorStub();
			using (ISession s = OpenSession(interceptor2))
			using (ITransaction tx = s.BeginTransaction())
			{
				Person jimmy = s.Get<Person>("Jimmy Hendrix");
				jimmy.Address = new Address();
				interceptor.entityToCheck = jimmy;
				tx.Commit();
			}
			Assert.IsFalse(interceptor2.entityWasDeemedDirty);
		}

		[Test]
		public void ReplaceValueTypeComponentWithSameValueDoesNotMakeItDirty()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person p = new Person("Jimmy Hendrix");
				Address a = new Address();
				a.Street = "Some Street";
				a.City = "Some City";
				p.Address = a;

				s.Save(p);
				tx.Commit();
			}

			InterceptorStub interceptor = new InterceptorStub();
			using (ISession s = OpenSession(interceptor))
			using (ITransaction tx = s.BeginTransaction())
			{
				Person jimmy = s.Get<Person>("Jimmy Hendrix");
				interceptor.entityToCheck = jimmy;

				Address a = new Address();
				a.Street = "Some Street";
				a.City = "Some City";
				jimmy.Address = a;
				Assert.AreNotSame(jimmy.Address, a);

				tx.Commit();
			}
			Assert.IsFalse(interceptor.entityWasDeemedDirty);
		}
	}

	public class InterceptorStub : EmptyInterceptor
	{
		public object entityToCheck;
		public bool entityWasDeemedDirty = false;

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, NHibernate.Type.IType[] types)
		{
			if (entity == entityToCheck) { entityWasDeemedDirty = true; }

			return false;
		}
	}
}
