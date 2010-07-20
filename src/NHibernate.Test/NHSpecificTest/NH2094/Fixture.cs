using NHibernate.ByteCode.Castle;
using NHibernate.Cfg;
using NUnit.Framework;
 
namespace NHibernate.Test.NHSpecificTest.NH2094
{
  [TestFixture]
  public class Fixture : BugTestCase
  {
    protected override void Configure(Configuration configuration)
    {
      configuration.SetProperty(Environment.ProxyFactoryFactoryClass,
                    typeof(ProxyFactoryFactory).AssemblyQualifiedName);
    }
 
    [Test]
    public void CanAccessInitializedPropertiesOutsideOfSession()
    {
      try
      {
        using (var s = OpenSession())
        {
          var p = new Person { Id = 1, Name = "Person1", LazyField = "Long field"};
 
          s.Save(p);

          s.Flush();
        }

        Person person;

        using (var s = OpenSession())
        {
          person = s.Get<Person>(1);

          Assert.AreEqual("Person1", person.Name);
          Assert.AreEqual("Long field", person.LazyField);
        }

        Assert.AreEqual("Person1", person.Name);
        Assert.AreEqual("Long field", person.LazyField);
      }
      finally
      {
        using (var s = OpenSession())
        {
          s.Delete("from Person");

          s.Flush();
        }
      }
    }
  }
}
