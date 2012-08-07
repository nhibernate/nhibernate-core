using System;
using System.Linq;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2819
{
	public class Fixture2819 : BugTestCase
	{
		[Test]
		public void should_be_able_to_call_generic_methods_on_a_proxy()
		{
			var addressId = Guid.Empty;
			using (ISession session = sessions.OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					
					var address = new Address();

					session.SaveOrUpdate(address);
					tx.Commit();
					addressId = address.Id;
				}
			}

			using (ISession session = sessions.OpenSession())
			{
				using (session.BeginTransaction())
				{
					var address = session.Load<Address>(addressId);

					Assert.That(address, Is.AssignableTo<INHibernateProxy>());

					// call to generic method on a proxy will fail on .Net 4.0
					var res = address.GenericMethod<int>(42);
					Assert.That(res, Is.EqualTo(42));
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession session = sessions.OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					foreach (var address in session.QueryOver<Address>().List())
					{
						session.Delete(address);
					}
					tx.Commit();
				}
			}
		}
	}
}
