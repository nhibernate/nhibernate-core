using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1421
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void WhenParameterListIsEmptyArrayUsingQueryThenDoesNotTrowsNullReferenceException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				Assert.That(() => query.SetParameterList("myList", Array.Empty<long>()), Throws.Exception.Not.InstanceOf<NullReferenceException>());
			}
		}

		[Test]
		public void WhenParameterListIsEmptyGenericCollectionUsingQueryThenDoesNotTrowsNullReferenceException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				Assert.That(() => query.SetParameterList("myList", new Collection<long>()), Throws.Exception.Not.InstanceOf<NullReferenceException>());
			}
		}

		[Test]
		public void WhenParameterListIsEmptyCollectionUsingQueryThenTrowsArgumentException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");

				var ex = Assert.Throws<QueryException>(() => query.SetParameterList("myList", new List<object>()));
				Assert.That(ex.Message, Is.EqualTo("An empty parameter-list generates wrong SQL; parameter name 'myList'"));
			}
		}

		[Test]
		public void WhenParameterListIsNullUsingQueryThenTrowsArgumentException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				Assert.That(() => query.SetParameterList("myList", null), Throws.InstanceOf<ArgumentNullException>());
			}
		}

		[Test]
		public void WhenParameterListIsEmptyUsingQueryThenDoesNotTrowsNullReferenceException()
		{
			using (var s = OpenSession())
			{
				var query = s.CreateQuery("from AnEntity a where a.id in (:myList)");
				Assert.That(() => query.SetParameterList("myList", Array.Empty<long>()).List(), Throws.Exception.Not.InstanceOf<NullReferenceException>());
			}
		}
	}
}
