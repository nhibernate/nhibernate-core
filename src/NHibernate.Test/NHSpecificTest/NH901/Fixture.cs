using System.Collections;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH901
{
	public abstract class FixtureBase : TestCase
	{
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

	[TestFixture]
	public class Fixture : FixtureBase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"NHSpecificTest.NH901.Mappings.hbm.xml"}; }
		}
	}

	[TestFixture]
	public class FixtureByCode : FixtureBase
	{
		protected override IList Mappings
		{
			get { return new string[0]; }
		}

		protected override string MappingsAssembly
		{
			get { return null; }
		}

		protected override void AddMappings(Configuration configuration)
		{
			var mapper = new ModelMapper();
			mapper.Class<Person>(rc =>
			{
				rc.Table("NH901_Person");
				rc.Id(x => x.Name, m => m.Generator(Generators.Assigned));
				rc.Component(x => x.Address, cm =>
				{
					cm.Property(x => x.City);
					cm.Property(x => x.Street);
				});
			});
			configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());
		}
	}

	public class InterceptorStub : EmptyInterceptor
	{
		public object entityToCheck;
		public bool entityWasDeemedDirty = false;

		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
			if (entity == entityToCheck) { entityWasDeemedDirty = true; }

			return false;
		}
	}
}
