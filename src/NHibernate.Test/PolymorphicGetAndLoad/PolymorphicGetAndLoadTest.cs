using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Proxy;
using NUnit.Framework;

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
			Assert.That(() =>
			{
				using (var s = OpenSession())
				{
					s.Save(a);
					s.Flush();
				}
			}, Throws.Nothing);

			Assert.That(() =>
			{
				using (var s = OpenSession())
				{
					s.Delete(a);
					s.Flush();
				}
			}, Throws.Nothing);

		}

		[Test]
		public void WhenLoadBaseClassUsingInterfaceThenNotThrows()
		{
			using (var scenario = new ScenarioWithA(Sfi))
			{
				using (var s = OpenSession())
				{
					Assert.That(() => s.Load<INamed>(scenario.A.Id), Throws.Nothing);
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
					Assert.That(() => s.Get<INamed>(scenario.A.Id), Throws.Nothing);
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
					Assert.That(() => s.Load<INamed>(scenario.B.Id), Throws.Nothing);
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
					Assert.That(() => loadedEntity = s.Load<INamed>(scenario.B.Id), Throws.Nothing);
					Assert.That(NHibernateProxyHelper.GetClassWithoutInitializingProxy(loadedEntity), Is.EqualTo(typeof(A)));

					var narrowedProxy = s.Load<B>(scenario.B.Id);

					Assert.That(NHibernateProxyHelper.GetClassWithoutInitializingProxy(narrowedProxy), Is.EqualTo(typeof(B)));

					var firstLoadedImpl = ((INHibernateProxy)loadedEntity).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					var secondLoadedImpl = ((INHibernateProxy)narrowedProxy).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					Assert.That(firstLoadedImpl, Is.SameAs(secondLoadedImpl));
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
					Assert.That(() => loadedEntity = s.Load<INamed>(scenario.B.Id), Throws.Nothing);
					Assert.That(NHibernateProxyHelper.GetClassWithoutInitializingProxy(loadedEntity), Is.EqualTo(typeof(A)));

					var narrowedProxy = s.Load<IOccuped>(scenario.B.Id);

					Assert.That(NHibernateProxyHelper.GetClassWithoutInitializingProxy(narrowedProxy), Is.EqualTo(typeof(B)));

					var firstLoadedImpl = ((INHibernateProxy)loadedEntity).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					var secondLoadedImpl = ((INHibernateProxy)narrowedProxy).HibernateLazyInitializer.GetImplementation((ISessionImplementor)s);
					Assert.That(firstLoadedImpl, Is.SameAs(secondLoadedImpl));
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
					Assert.That(() => loadedEntity = s.Get<INamed>(scenario.B.Id), Throws.Nothing);
					Assert.That(loadedEntity, Is.TypeOf<B>());
				}
			}
		}

		[Test]
		public void WhenLoadClassUsingInterfaceOfMultippleHierarchyThenThrows()
		{
			using (var s = OpenSession())
			{
				Assert.That(() => s.Load<IMultiGraphNamed>(1), Throws.TypeOf<HibernateException>()
																	 .And.Message.ContainsSubstring("Ambiguous")
																	 .And.Message.ContainsSubstring("GraphA")
																	 .And.Message.ContainsSubstring("GraphB")
																	 .And.Message.ContainsSubstring("IMultiGraphNamed"));
			}
		}

		[Test]
		public void WhenGetClassUsingInterfaceOfMultippleHierarchyThenThrows()
		{
			using (var s = OpenSession())
			{
				Assert.That(() => s.Get<IMultiGraphNamed>(1),
							Throws.TypeOf<HibernateException>()
								  .And.Message.StringContaining("Ambiguous")
								  .And.Message.StringContaining("GraphA")
								  .And.Message.StringContaining("GraphB")
								  .And.Message.StringContaining("IMultiGraphNamed"));
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
					Assert.That(() => s.Get<INamed>(id), Throws.Nothing);
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
					Assert.That(() => s.Get<INamed>(id), Throws.Nothing);
				}
			}
		}
	}
}