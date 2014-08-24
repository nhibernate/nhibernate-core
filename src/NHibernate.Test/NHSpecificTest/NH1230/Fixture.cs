using NHibernate.Cfg;
using NHibernate.Event;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1230
{
	[TestFixture,Ignore("TODO(Dario)This test demostrate the need of eliminate the 'bool' on pre-insert eventlisteners.")]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1230"; }
		}

		protected override void Configure(Configuration cfg)
		{
			cfg.SetListener(ListenerType.PreInsert, new PreSaveDoVeto());
		}

		/// <summary>
		/// Must be vetoed without thrown an <see cref="AssertionFailure"/> exception.
		/// As no identifier generation has made, the id is null and is not posible create a EntityKey instance to agregate
		/// to the PersistenceContext (don't know why registrate a vetoed entity to PC, that's the reason of make the method: void
		/// </summary>
		[Test]
		public void NoExceptionMustBeThrown1()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					FooIdentity f = new FooIdentity();
					f.Description = "f1";
					f.Id = 1;
					s.Save(f);
					tx.Commit();
				}
			}
		}

		/// <summary>
		/// Must be vetoed without thrown an <see cref="NonUniqueObjectException"/> exception.
		/// The second time the entity must be vetoed again, but not registration on PersistenceContext must added.
		/// The identity map contains the first that has vetoed.
		/// </summary>
		[Test]
		public void NoExceptionMustBeThrown2()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					FooAssigned f = new FooAssigned();
					f.Description = "f1";
					f.Id = 1;
					s.Save(f);
					tx.Commit();
				}

				using (ITransaction tx = s.BeginTransaction())
				{
					FooAssigned f = new FooAssigned();
					f.Description = "f2";
					f.Id = 1;
					s.Save(f);
					tx.Commit();
				}
			}
		}
	}
}