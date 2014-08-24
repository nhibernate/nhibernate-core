using System.Collections;
using System.Collections.Generic;
using NHibernate.Event;
using NHibernate.Impl;
using NUnit.Framework;

namespace NHibernate.Test.Events.PostEvents
{
	[TestFixture]
	public class PostUpdateFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new[] {"Events.PostEvents.SimpleEntity.hbm.xml"}; }
		}

		[Test]
		public void ImplicitFlush()
		{
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[]
			                                                                          	{
			                                                                          		new AssertOldStatePostListener(
			                                                                          			eArgs =>
			                                                                          			Assert.That(eArgs.OldState, Is.Not.Null))
			                                                                          	};
			FillDb();
			using (var ls = new LogSpy(typeof (AssertOldStatePostListener)))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction tx = s.BeginTransaction())
					{
						IList<SimpleEntity> l = s.CreateCriteria<SimpleEntity>().List<SimpleEntity>();
						l[0].Description = "Modified";
						tx.Commit();
					}
				}
				Assert.That(ls.GetWholeLog(), Is.StringContaining(AssertOldStatePostListener.LogMessage));
			}

			DbCleanup();
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[0];
		}

		[Test]
		public void ExplicitUpdate()
		{
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[]
			                                                                          	{
			                                                                          		new AssertOldStatePostListener(
			                                                                          			eArgs =>
			                                                                          			Assert.That(eArgs.OldState, Is.Not.Null))
			                                                                          	};
			FillDb();
			using (var ls = new LogSpy(typeof (AssertOldStatePostListener)))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction tx = s.BeginTransaction())
					{
						IList<SimpleEntity> l = s.CreateCriteria<SimpleEntity>().List<SimpleEntity>();
						l[0].Description = "Modified";
						s.Update(l[0]);
						tx.Commit();
					}
				}
				Assert.That(ls.GetWholeLog(), Is.StringContaining(AssertOldStatePostListener.LogMessage));
			}

			DbCleanup();
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[0];
		}

		[Test]
		public void WithDetachedObject()
		{
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[]
			                                                                          	{
			                                                                          		new AssertOldStatePostListener(
			                                                                          			eArgs =>
			                                                                          			Assert.That(eArgs.OldState, Is.Not.Null))
			                                                                          	};
			FillDb();
			SimpleEntity toModify;
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					IList<SimpleEntity> l = s.CreateCriteria<SimpleEntity>().List<SimpleEntity>();
					toModify = l[0];
					tx.Commit();
				}
			}
			toModify.Description = "Modified";
			using (var ls = new LogSpy(typeof (AssertOldStatePostListener)))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction tx = s.BeginTransaction())
					{
						s.Merge(toModify);
						tx.Commit();
					}
				}
				Assert.That(ls.GetWholeLog(), Is.StringContaining(AssertOldStatePostListener.LogMessage));
			}

			DbCleanup();
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[0];
		}

		[Test]
		public void UpdateDetachedObject()
		{
			// When the update is used directly as method to reattach a entity the OldState is null
			// that mean that NH should not retrieve info from DB
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[]
			                                                                          	{
			                                                                          		new AssertOldStatePostListener(
			                                                                          			eArgs =>
			                                                                          			Assert.That(eArgs.OldState, Is.Null))
			                                                                          	};
			FillDb();
			SimpleEntity toModify;
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					IList<SimpleEntity> l = s.CreateCriteria<SimpleEntity>().List<SimpleEntity>();
					toModify = l[0];
					tx.Commit();
				}
			}
			toModify.Description = "Modified";
			using (var ls = new LogSpy(typeof (AssertOldStatePostListener)))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction tx = s.BeginTransaction())
					{
						s.Update(toModify);
						tx.Commit();
					}
				}
				Assert.That(ls.GetWholeLog(), Is.StringContaining(AssertOldStatePostListener.LogMessage));
			}

			DbCleanup();
			((SessionFactoryImpl) sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[0];
		}

		[Test]
		public void UpdateDetachedObjectWithLock()
		{
			((SessionFactoryImpl)sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[]
			                                                                          	{
			                                                                          		new AssertOldStatePostListener(
			                                                                          			eArgs =>
			                                                                          			Assert.That(eArgs.OldState, Is.Not.Null))
			                                                                          	};
			FillDb();
			SimpleEntity toModify;
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					IList<SimpleEntity> l = s.CreateCriteria<SimpleEntity>().List<SimpleEntity>();
					toModify = l[0];
					tx.Commit();
				}
			}
			using (var ls = new LogSpy(typeof(AssertOldStatePostListener)))
			{
				using (ISession s = OpenSession())
				{
					using (ITransaction tx = s.BeginTransaction())
					{
						s.Lock(toModify, LockMode.None);
						toModify.Description = "Modified";
						s.Update(toModify);
						tx.Commit();
					}
				}
				Assert.That(ls.GetWholeLog(), Is.StringContaining(AssertOldStatePostListener.LogMessage));
			}

			DbCleanup();
			((SessionFactoryImpl)sessions).EventListeners.PostUpdateEventListeners = new IPostUpdateEventListener[0];
		}
		private void DbCleanup()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.CreateQuery("delete from SimpleEntity").ExecuteUpdate();
					tx.Commit();
				}
			}
		}

		private void FillDb()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					s.Save(new SimpleEntity {Description = "Something"});
					tx.Commit();
				}
			}
		}
	}
}