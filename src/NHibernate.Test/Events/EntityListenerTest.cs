using System.Linq;
using NHibernate.Event;
using NUnit.Framework;

namespace NHibernate.Test.Events
{
    [TestFixture]
    public class EntityListenerTest
    {
        private readonly EntityListener _listener = new EntityListener();

        private void AssertCalledMethodsAre(MyEntityForTesting entity, params string[] names)
        {
            Assert.AreEqual(names.Count(), entity.CalledMethods.Count);
            foreach (string name in names)
            {
                Assert.IsTrue(entity.CalledMethods.Contains(name), "Expected method '" + name + "' to be called but it wasn't!");
            }
        }

        [Test]
        public void CallEventMultipleTimes_CachingOk()
        {
            var entity = new MyEntityForTesting();
            var postDeleteEvent = new PostDeleteEvent(entity, "123", new object[] {  },new EntityPersisterStub(), null);
            _listener.OnPostDelete(postDeleteEvent);
            _listener.OnPostDelete(postDeleteEvent);
            _listener.OnPostInsert(new PostInsertEvent(entity, "123", new object[] {}, new EntityPersisterStub(), null));

            AssertCalledMethodsAre(entity, "PostDelete", "PostDelete(session)", "PostDelete", "PostDelete(session)", "PostInsert", "PostInsert(session)");
        }

        [Test]
        public void OnPostDelete_DoesNotAlterPropertyState()
        {
            var entity = new MyEntityForTesting();
            var state = new object[] {"oldstate"};
            _listener.OnPostDelete(new PostDeleteEvent(entity, "123", state,
                                                       new EntityPersisterStub {PropertyNames = entity.PropertyNames},
                                                       null));

            Assert.AreEqual("oldstate", state[0]);
        }

        [Test]
        public void OnPostDelete_TriggersPostDeleteMethod()
        {
            var entity = new MyEntityForTesting();
            _listener.OnPostDelete(new PostDeleteEvent(entity, "123", new object[] {}, new EntityPersisterStub(), null));

            AssertCalledMethodsAre(entity, "PostDelete", "PostDelete(session)");
        }

        [Test]
        public void OnPostInsert_DoesNotUpdatePropertyState()
        {
            var entity = new MyEntityForTesting();
            var persister = new EntityPersisterStub {PropertyNames = entity.PropertyNames};
            var state = new object[] {"oldstate"};

            _listener.OnPostInsert(new PostInsertEvent(entity, "123", state, persister, null));

            Assert.AreEqual("oldstate", state[0]);
        }

        [Test]
        public void OnPostInsert_TriggersPostInsertMethod()
        {
            var entity = new MyEntityForTesting();
            _listener.OnPostInsert(new PostInsertEvent(entity, "123", new object[] {}, new EntityPersisterStub(), null));

            AssertCalledMethodsAre(entity, "PostInsert", "PostInsert(session)");
        }

        [Test]
        public void OnPostUpdate_DoesNotUpdatePropertyState()
        {
            var entity = new MyEntityForTesting();
            var persister = new EntityPersisterStub {PropertyNames = entity.PropertyNames};
            var state = new object[] {"oldstate"};

            _listener.OnPostUpdate(new PostUpdateEvent(entity, "123", state, new object[] {}, persister, null));

            Assert.AreEqual("oldstate", state[0]);
        }

        [Test]
        public void OnPostUpdate_TriggersPostUpdateMethod()
        {
            var entity = new MyEntityForTesting();
            _listener.OnPostUpdate(new PostUpdateEvent(entity, "123", new object[] {}, new object[] {},
                                                       new EntityPersisterStub(), null));

            AssertCalledMethodsAre(entity, "PostUpdate", "PostUpdate(session)");
        }

        [Test]
        public void OnPreDelete_DoesNotAlterPropertyState()
        {
            var entity = new MyEntityForTesting();
            var state = new object[] {"oldstate"};
            _listener.OnPreDelete(new PreDeleteEvent(entity, "123", state,
                                                     new EntityPersisterStub {PropertyNames = entity.PropertyNames},
                                                     null));

            Assert.AreEqual("oldstate", state[0]);
        }

        [Test]
        public void OnPreDelete_TriggersPreDeleteMethod()
        {
            var entity = new MyEntityForTesting();
            _listener.OnPreDelete(new PreDeleteEvent(entity, "123", new object[] {}, new EntityPersisterStub(), null));

            AssertCalledMethodsAre(entity, "PreDelete", "PreDelete(session)");
        }

        [Test]
        public void OnPreInsert_AlsoUpdatesPropertyState()
        {
            var entity = new MyEntityForTesting();
            var persister = new EntityPersisterStub {PropertyNames = entity.PropertyNames};
            var state = new object[] {"oldstate"};

            _listener.OnPreInsert(new PreInsertEvent(entity, "123", state, persister, null));

            Assert.AreEqual("preinsert", state[0]);
        }

        [Test]
        public void OnPreInsert_TriggersPreInsertMethod()
        {
            var entity = new MyEntityForTesting();
            _listener.OnPreInsert(new PreInsertEvent(entity, "123", new object[] {}, new EntityPersisterStub(), null));

            AssertCalledMethodsAre(entity, "PreInsert", "PreInsert(session)");
        }

        [Test]
        public void OnPreUpdate_AlsoUpdatesPropertyState()
        {
            var entity = new MyEntityForTesting();
            var persister = new EntityPersisterStub {PropertyNames = entity.PropertyNames};
            var state = new object[] {"oldstate"};

            _listener.OnPreUpdate(new PreUpdateEvent(entity, "123", state, new object[] {}, persister, null));

            Assert.AreEqual("preupdate", state[0]);
        }

        [Test]
        public void OnPreUpdate_TriggersPreUpdateMethod()
        {
            var entity = new MyEntityForTesting();
            _listener.OnPreUpdate(new PreUpdateEvent(entity, "123", new object[] {}, new object[] {},
                                                     new EntityPersisterStub(), null));

            AssertCalledMethodsAre(entity, "PreUpdate", "PreUpdate(session)");
        }
    }
}