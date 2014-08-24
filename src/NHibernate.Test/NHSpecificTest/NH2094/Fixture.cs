using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2094
{
  [TestFixture]
  public class Fixture : BugTestCase
  {
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

		[Test]
		public void WhenAccessNoLazyPropertiesOutsideOfSessionThenNotThrows()
		{
			try
			{
				using (var s = OpenSession())
				{
					var p = new Person { Id = 1, Name = "Person1", LazyField = "Long field" };

					s.Save(p);

					s.Flush();
				}

				Person person;

				using (var s = OpenSession())
				{
					person = s.Get<Person>(1);
				}
				string personName;
				Executing.This(()=> personName = person.Name).Should().NotThrow();
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

		[Test]
		public void WhenAccessLazyPropertiesOutsideOfSessionThenThrows()
		{
			try
			{
				using (var s = OpenSession())
				{
					var p = new Person { Id = 1, Name = "Person1", LazyField = "Long field" };

					s.Save(p);

					s.Flush();
				}

				Person person;

				using (var s = OpenSession())
				{
					person = s.Get<Person>(1);
				}
				string lazyField;
				var lazyException = Executing.This(() => lazyField = person.LazyField).Should().Throw<LazyInitializationException>().Exception;
				lazyException.EntityName.Should().Not.Be.Null();
				lazyException.Message.Should().Contain("LazyField");
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
