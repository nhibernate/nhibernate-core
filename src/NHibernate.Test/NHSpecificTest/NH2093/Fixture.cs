using NHibernate.Proxy;
using NUnit.Framework;
 
namespace NHibernate.Test.NHSpecificTest.NH2093
 {
  [TestFixture]
  public class Fixture : BugTestCase
  {
    [Test]
    public void NHibernateProxyHelperReturnsCorrectType()
    {
      try
      {
        using (var s = OpenSession())
        {
          var person = new Person { Id = 1, Name = "Person1" };
          var employee = new Employee { Id = 1, Name = "Emp1", Person = person };

          s.Save(person);
          s.Save(employee);

          s.Flush();
        }

        using (var s = OpenSession())
        {
          var person = s.Load<Person>(1);

          var type = NHibernateProxyHelper.GuessClass(person);

          Assert.AreEqual(type, typeof(Person));
        }

        using (var s = OpenSession())
        {
          var person = s.Get<Person>(1);

          var type = NHibernateProxyHelper.GuessClass(person);

          Assert.AreEqual(type, typeof(Person));
        }
      }
      finally
      {
        using (var s = OpenSession())
        {
          s.Delete("from Employee");
          s.Delete("from Person");

          s.Flush();
        }
      }
    }

    [Test]
    public void CanUseFieldInterceptingProxyAsHQLArgument()
    {
      try
      {
        using (var s = OpenSession())
        {
          var person = new Person { Id = 1, Name = "Person1" };
          var employee = new Employee { Id = 1, Name = "Emp1", Person = person };

          s.Save(person);
          s.Save(employee);

          s.Flush();
        }

        using (var s = OpenSession())
        {
          var person = s.Get<Person>(1);

          var list = s.CreateQuery("from Employee where Person = :p")
            .SetEntity("p", person)
            .List<Employee>();

          Assert.AreEqual(list.Count, 1);
        }
      }
      finally
      {
        using (var s = OpenSession())
        {
          s.Delete("from Employee");
          s.Delete("from Person");

          s.Flush();
        }
      }
    }
  }
 }
