using System;
using System.Collections;
using NUnit.Framework;
using SharpTestsEx;
using System.Collections.ObjectModel;

namespace NHibernate.Test.NHSpecificTest.NH1421
{
	public class Fixture: BugTestCase
	{
		[Test]
		public void WhenParameterListIsEmptyArrayUsingQueryThenDoesNotTrowsNullReferenceException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				query.Executing(x => x.SetParameterList("myList", new long[0])).Throws().And.Exception.Should().Not.Be.InstanceOf<NullReferenceException>();
			}
		}

		[Test]
		public void WhenParameterListIsEmptyGenericCollectionUsingQueryThenDoesNotTrowsNullReferenceException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				query.Executing(x => x.SetParameterList("myList", new Collection<long>())).Throws().And.Exception.Should().Not.Be.InstanceOf<NullReferenceException>();
			}
		}

		[Test]
		public void WhenParameterListIsEmptyCollectionUsingQueryThenTrowsArgumentException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				query.Executing(x => x.SetParameterList("myList", new ArrayList())).Throws().And.Exception.Should().Be.InstanceOf<ArgumentException>();
			}
		}

		[Test]
		public void WhenParameterListIsNullUsingQueryThenTrowsArgumentException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				query.Executing(x => x.SetParameterList("myList", null)).Throws().And.Exception.Should().Be.InstanceOf<ArgumentNullException>();
			}
		}

		[Test]
		public void WhenParameterListIsEmptyUsingQueryThenDoesNotTrowsNullReferenceException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				query.Executing(x => x.SetParameterList("myList", new long[0]).List()).Throws().And.Exception.Should().Not.Be.InstanceOf<NullReferenceException>();
			}
		}

	}
}