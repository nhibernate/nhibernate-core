﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Proxy;
using NUnit.Framework;
 
namespace NHibernate.Test.NHSpecificTest.NH2093
 {
   using System.Threading.Tasks;
  [TestFixture]
  public class FixtureAsync : BugTestCase
  {
    [Test]
    public async Task NHibernateProxyHelperReturnsCorrectTypeAsync()
    {
      try
      {
        using (var s = OpenSession())
        {
          var person = new Person { Id = 1, Name = "Person1" };
          var employee = new Employee { Id = 1, Name = "Emp1", Person = person };

          await (s.SaveAsync(person));
          await (s.SaveAsync(employee));

          await (s.FlushAsync());
        }

        using (var s = OpenSession())
        {
          var person = await (s.LoadAsync<Person>(1));

          var type = NHibernateProxyHelper.GuessClass(person);

          Assert.AreEqual(type, typeof(Person));
        }

        using (var s = OpenSession())
        {
          var person = await (s.GetAsync<Person>(1));

          var type = NHibernateProxyHelper.GuessClass(person);

          Assert.AreEqual(type, typeof(Person));
        }
      }
      finally
      {
        using (var s = OpenSession())
        {
          await (s.DeleteAsync("from Employee"));
          await (s.DeleteAsync("from Person"));

          await (s.FlushAsync());
        }
      }
    }

    [Test]
    public async Task CanUseFieldInterceptingProxyAsHQLArgumentAsync()
    {
      try
      {
        using (var s = OpenSession())
        {
          var person = new Person { Id = 1, Name = "Person1" };
          var employee = new Employee { Id = 1, Name = "Emp1", Person = person };

          await (s.SaveAsync(person));
          await (s.SaveAsync(employee));

          await (s.FlushAsync());
        }

        using (var s = OpenSession())
        {
          var person = await (s.GetAsync<Person>(1));

          var list = await (s.CreateQuery("from Employee where Person = :p")
            .SetEntity("p", person)
            .ListAsync<Employee>());

          Assert.AreEqual(1, list.Count);
        }
      }
      finally
      {
        using (var s = OpenSession())
        {
          await (s.DeleteAsync("from Employee"));
          await (s.DeleteAsync("from Person"));

          await (s.FlushAsync());
        }
      }
    }
  }
 }
