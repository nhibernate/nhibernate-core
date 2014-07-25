using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Linq;

namespace NHibernate.Test.NHSpecificTest.NH3614
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CanProjectListOfStrings()
		{
			Guid id;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var testEntity = new Entity
				{
					SomeStrings = new List<string> { "Hello", "World" }
				};
				s.Save(testEntity);

				tx.Commit();

				id = testEntity.Id;
			}

			using (var s = OpenSession())
			{
				var result = s.Query<Entity>()
					.Where(x => x.Id == id)
					.Select(x => x.SomeStrings)
					.ToList();

				Assert.AreEqual(1, result.Count);

				Assert.AreEqual(2, result.Single().Count);
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Entity");
				tx.Commit();
			}
		}
	}
}
