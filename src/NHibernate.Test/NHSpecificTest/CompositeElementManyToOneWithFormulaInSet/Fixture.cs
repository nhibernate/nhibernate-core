using System;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.CompositeElementManyToOneWithFormulaInSet
{
    [TestFixture]
    public class WeirdBehaviour : BugTestCase
    {
        #region Instance Variables (1)

        private Guid nicePersonId;

        #endregion Instance Variables

        #region Public Methods (6)


        [Test]
        public void CanAddATodo()
        {

            using (ISession session = OpenSession())
            {
                var person = new Person("myName");                
                person.AddTodo(new Todo(person) { Name = "I need to get it" });
                session.Save(person);
                nicePersonId = person.Id;
                Assert.AreEqual(1, person.Todos.Count());
                session.Flush();
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(1, person.Todos.Count());
                Assert.AreEqual(person.Todos.ToList()[0].Person.Id, person.Id);
            }
        }

        [Test]
        public void CanRemoveATodo()
        {
            Todo myTodo;
            using (ISession session = OpenSession())
            {
                var person = new Person("myName2");                
                myTodo = person.AddTodo(new Todo(person) { Name = "I need to get it" });
                session.Save(person);
                nicePersonId = person.Id;
                Assert.AreEqual(1, person.Todos.Count());
                session.Flush();
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(1, person.Todos.Count());
                person.RemoveTodo(myTodo);
                Assert.AreEqual(0, person.Todos.Count());
                session.Flush();
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(0, person.Todos.Count());
            }
        }

        [Test]
        public void CanAddStuff()
        {
            using (ISession session = OpenSession())
            {
                var person = new Person("myName3");
                person.AddStuff(new Stuff(person) { Name = "this pen is mine" });
                session.Save(person);
                nicePersonId = person.Id;
                Assert.AreEqual(1, person.MyStuff.Count());
                session.Flush();
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(1, person.MyStuff.Count());
                Assert.AreEqual(person.MyStuff.ToList()[0].Person.Id, person.Id);
            }
        }


        [Test]
        public void CanRemoveStuff()
        {
            Stuff myStuff;
            using (ISession session = OpenSession())
            {
                var person = new Person("MyName4");
                myStuff = person.AddStuff(new Stuff(person) { Name = "BallPen" });
                session.Save(person);
                nicePersonId = person.Id;
                Assert.AreEqual(1, person.MyStuff.Count());
                session.Flush();
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(1, person.MyStuff.Count());
                person.RemoveStuff(myStuff);
                Assert.AreEqual(0, person.MyStuff.Count());
                session.Flush();
            }
            using (ISession session = OpenSession())
            {
                var person = session.Get<Person>(nicePersonId);
                Assert.AreEqual(0, person.MyStuff.Count());
            }
        }

        #endregion Public Methods

        #region Protected Methods (2)

        protected override void OnSetUp()
        {
            base.OnSetUp();
            Console.WriteLine("=====================BEGIN TEST");
        }

        protected override void OnTearDown()
        {
            Console.WriteLine("=====================END TEST");
            base.OnTearDown();
            using (ISession session = OpenSession())
            {
                const string hql = "from System.Object";
                session.Delete(hql);
                session.Flush();
            }
        }

        #endregion Protected Methods
    }
}