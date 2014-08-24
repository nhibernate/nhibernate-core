using System;
using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2488
{
	public class Fixture : BugTestCase
	{
		#region Scenarios

		private class FetchSelectScenario: IDisposable
		{
			private readonly ISessionFactory factory;

			public FetchSelectScenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var entity = new Derived1
						             {
						             	ShortContent = "Short",
						             	LongContent = "LongLongLongLongLong",
						             };
						s.Save(entity);
						t.Commit();
					}
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						s.Delete("from Derived1");
						t.Commit();
					}
				}
			}
		}
		private class FetchJoinScenario : IDisposable
		{
			private readonly ISessionFactory factory;

			public FetchJoinScenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var entity = new Derived2
						             {
						             	ShortContent = "Short",
						             	LongContent = "LongLongLongLongLong",
						             };
						s.Save(entity);
						t.Commit();
					}
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						s.Delete("from Derived2");
						t.Commit();
					}
				}
			}
		}
		private class JoinedSubclassScenario : IDisposable
		{
			private readonly ISessionFactory factory;

			public JoinedSubclassScenario(ISessionFactory factory)
			{
				this.factory = factory;
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						var entity = new Derived3
						{
							ShortContent = "Short",
							LongContent = "LongLongLongLongLong",
						};
						s.Save(entity);
						t.Commit();
					}
				}
			}

			public void Dispose()
			{
				using (ISession s = factory.OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						s.Delete("from Derived3");
						t.Commit();
					}
				}
			}
		}

		#endregion

		[Test]
		public void ShouldNotQueryLazyProperties_FetchJoin()
		{
			using (new FetchJoinScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						IList<Base2> items;
						using (var ls = new SqlLogSpy())
						{
							items = s.CreateQuery("from Base2").List<Base2>();
							ls.GetWholeLog().Should().Not.Contain("LongContent");
						}
						var item = (Derived2) items[0];
						NHibernateUtil.IsPropertyInitialized(item, "LongContent").Should().Be.False();
						string lc = item.LongContent;
						lc.Should().Not.Be.NullOrEmpty();
						NHibernateUtil.IsPropertyInitialized(item, "LongContent").Should().Be.True();
					}
				}
			}
		}

		[Test]
		public void ShouldNotQueryLazyProperties_FetchSelect()
		{
			using (new FetchSelectScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						IList<Base1> items;
						using(var ls = new SqlLogSpy())
						{
							items = s.CreateQuery("from Base1").List<Base1>();
							ls.GetWholeLog().Should().Not.Contain("LongContent");
						}
						var item = (Derived1) items[0];
						NHibernateUtil.IsPropertyInitialized(item, "LongContent").Should().Be.False();
						string lc = item.LongContent;
						lc.Should().Not.Be.NullOrEmpty();
						NHibernateUtil.IsPropertyInitialized(item, "LongContent").Should().Be.True();
					}
				}
			}
		}

		[Test]
		public void ShouldNotQueryLazyProperties_Joinedsubclass()
		{
			using (new JoinedSubclassScenario(Sfi))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction t = s.BeginTransaction())
					{
						IList<Base3> items;
						using (var ls = new SqlLogSpy())
						{
							items = s.CreateQuery("from Base3").List<Base3>();
							ls.GetWholeLog().Should().Not.Contain("LongContent");
						}
						var item = (Derived3)items[0];
						NHibernateUtil.IsPropertyInitialized(item, "LongContent").Should().Be.False();
						string lc = item.LongContent;
						lc.Should().Not.Be.NullOrEmpty();
						NHibernateUtil.IsPropertyInitialized(item, "LongContent").Should().Be.True();
					}
				}
			}
		}
	}
}