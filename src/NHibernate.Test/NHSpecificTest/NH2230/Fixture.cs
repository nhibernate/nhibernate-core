using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH2230
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		[Test]
		public void CanCreacteRetrieveDeleteComponentsWithPrivateReferenceSetterToParent()
		{
			var entity = new MyEntity();
			var component = new MyComponentWithParent(entity){Something = "A"};
			entity.Component = component;
			entity.Children = new List<MyComponentWithParent>
								{
									new MyComponentWithParent(entity){Something = "B"},
														new MyComponentWithParent(entity){Something = "C"}
								};
			object poid;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				poid = s.Save(entity);
				tx.Commit();
			}

			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var savedEntity = s.Get<MyEntity>(poid);
				var myComponentWithParent = savedEntity.Component;
				Assert.That(myComponentWithParent, Is.Not.Null);
				Assert.That(myComponentWithParent.Parent, Is.SameAs(savedEntity));
				Assert.That(myComponentWithParent.Something, Is.EqualTo("A"));

				Assert.That(savedEntity.Children.Select(c => c.Something), Is.EquivalentTo(new [] {"B", "C"}));
				Assert.That(savedEntity.Children.All(c => ReferenceEquals(c.Parent, savedEntity)), Is.True);

				s.Delete(savedEntity);
				tx.Commit();
			}
		}
	}
}