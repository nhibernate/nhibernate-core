using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.PolymorphicGetAndLoad
{
	public class PolymorphicGetAndLoadTest: TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] { "PolymorphicGetAndLoad.Mappings.hbm.xml" }; }
		}

		public class ScenarioWithA : IDisposable
		{
			private readonly ISessionFactory factory;
			private readonly A a;

			public ScenarioWithA(ISessionFactory factory)
			{
				this.factory = factory;
				a = new A { Name = "Patrick" };
				using (var s = factory.OpenSession())
				{
					s.Save(a);
					s.Flush();
				}
			}

			public A A
			{
				get { return a; }
			}

			public void Dispose()
			{
				using (var s = factory.OpenSession())
				{
					s.Delete(a);
					s.Flush();
				}
			}
		}

		public class ScenarioWithB : IDisposable
		{
			private readonly ISessionFactory factory;
			private readonly B b;

			public ScenarioWithB(ISessionFactory factory)
			{
				this.factory = factory;
				b = new B { Name = "Patrick", Occupation = "hincha pelotas (en el buen sentido), but good candidate to be committer."};
				using (var s = factory.OpenSession())
				{
					s.Save(b);
					s.Flush();
				}
			}

			public B B
			{
				get { return b; }
			}

			public void Dispose()
			{
				using (var s = factory.OpenSession())
				{
					s.Delete(b);
					s.Flush();
				}
			}
		}

		[Test]
		public void WhenSaveDeleteBaseClassCastedToInterfaceThenNotThrows()
		{
			INamed a = new A { Name = "Patrick" };
			Executing.This(() =>
											{
												using (var s = OpenSession())
												{
													s.Save(a);
													s.Flush();
												}
											}).Should().NotThrow();

			Executing.This(() =>
											{
												using (var s = OpenSession())
												{
													s.Delete(a);
													s.Flush();
												}
											}).Should().NotThrow();

		}

		[Test]
		public void WhenLoadBaseClassUsingInterfaceThenNotThrows()
		{
			using (var scenario = new ScenarioWithA(Sfi))
			{
				using (var s = OpenSession())
				{
					s.Executing(session => session.Load<INamed>(scenario.A.Id)).NotThrows();
				}
			}
		}

		[Test]
		public void WhenGetBaseClassUsingInterfaceThenNotThrows()
		{
			using (var scenario = new ScenarioWithA(Sfi))
			{
				using (var s = OpenSession())
				{
					s.Executing(session => session.Get<INamed>(scenario.A.Id)).NotThrows();
				}
			}
		}

		[Test]
		public void WhenLoadInheritedClassUsingInterfaceThenNotThrows()
		{
			using (var scenario = new ScenarioWithB(Sfi))
			{
				using (var s = OpenSession())
				{
					Executing.This(()=> s.Load<INamed>(scenario.B.Id) ).Should().NotThrow();
				}
			}
		}

		[Test]
		public void WhenLoadInheritedClassUsingInterfaceThenShouldAllowNarrowingProxy()
		{
			using (var scenario = new ScenarioWithB(Sfi))
			{
				using (var s = OpenSession())
				{
					INamed loadedEntity = null;
					Executing.This(() => loadedEntity = s.Load<INamed>(scenario.B.Id)).Should().NotThrow();
					NHibernateProxyHelper.GetClassWithoutInitializingProxy(loadedEntity).Should().Be(typeof(A));

					var narrowedProxy = s.Load<B>(scenario.B.Id);

					NHibernateProxyHelper.GetClassWithoutInitializingProxy(narrowedProxy).Should().Be(typeof(B));

					var firstLoadedImpl = ((INHibernateProxy)loadedEntity).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					var secondLoadedImpl = ((INHibernateProxy)narrowedProxy).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					firstLoadedImpl.Should().Be.SameInstanceAs(secondLoadedImpl);
				}
			}
		}

		[Test]
		public void WhenLoadInterfaceThenShouldAllowNarrowingProxy()
		{
			using (var scenario = new ScenarioWithB(Sfi))
			{
				using (var s = OpenSession())
				{
					INamed loadedEntity = null;
					Executing.This(() => loadedEntity = s.Load<INamed>(scenario.B.Id)).Should().NotThrow();
					NHibernateProxyHelper.GetClassWithoutInitializingProxy(loadedEntity).Should().Be(typeof(A));

					var narrowedProxy = s.Load<IOccuped>(scenario.B.Id);

					NHibernateProxyHelper.GetClassWithoutInitializingProxy(narrowedProxy).Should().Be(typeof(B));

					var firstLoadedImpl = ((INHibernateProxy)loadedEntity).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					var secondLoadedImpl = ((INHibernateProxy)narrowedProxy).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					firstLoadedImpl.Should().Be.SameInstanceAs(secondLoadedImpl);
				}
			}
		}

		[Test]
		public void WhenGetInheritedClassUsingInterfaceThenNotThrows()
		{
			using (var scenario = new ScenarioWithB(Sfi))
			{
				using (var s = OpenSession())
				{
					INamed loadedEntity = null;
					Executing.This(() => loadedEntity = s.Get<INamed>(scenario.B.Id)).Should().NotThrow();
					loadedEntity.Should().Be.OfType<B>();
				}
			}
		}

		[Test]
		public void WhenLoadClassUsingInterfaceOfMultippleHierarchyThenThrows()
		{
			using (var s = OpenSession())
			{
				Executing.This(() => s.Load<IMultiGraphNamed>(1))
					.Should().Throw<HibernateException>()
					.And.ValueOf.Message.Should()
					.Contain("Ambiguous")
					.And.Contain("GraphA")
					.And.Contain("GraphB")
					.And.Contain("IMultiGraphNamed");
			}
		}

		[Test]
		public void WhenGetClassUsingInterfaceOfMultippleHierarchyThenThrows()
		{
			using (var s = OpenSession())
			{
				Executing.This(() => s.Get<IMultiGraphNamed>(1))
					.Should().Throw<HibernateException>()
					.And.ValueOf.Message.Should()
					.Contain("Ambiguous")
					.And.Contain("GraphA")
					.And.Contain("GraphB")
					.And.Contain("IMultiGraphNamed");
			}
		}

		[Test]
		public void WhenGetBaseClassUsingInterfaceFromSessionCacheThenNotThrows()
		{
			using (var scenario = new ScenarioWithA(Sfi))
			{
				using (var s = OpenSession())
				{
					var id = scenario.A.Id;
					s.Get<A>(id);
					s.Executing(session => session.Get<INamed>(id)).NotThrows();
				}
			}
		}

		[Test]
		public void WhenGetInheritedClassUsingInterfaceFromSessionCacheThenNotThrows()
		{
			using (var scenario = new ScenarioWithB(Sfi))
			{
				using (var s = OpenSession())
				{
					var id = scenario.B.Id;
					s.Get<B>(id);
					s.Executing(session => session.Get<INamed>(id)).NotThrows();
				}
			}
		}
	}
}