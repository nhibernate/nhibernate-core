using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using SharpTestsEx;

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
				myComponentWithParent.Should().Not.Be.Null();
				myComponentWithParent.Parent.Should().Be.SameInstanceAs(savedEntity);
				myComponentWithParent.Something.Should().Be("A");

				savedEntity.Children.Select(c => c.Something).Should().Have.SameValuesAs("B", "C");
				savedEntity.Children.Select(child=> child.Parent).All(parent => parent.Satisfy(myEntity => ReferenceEquals(myEntity, savedEntity)));

				s.Delete(savedEntity);
				tx.Commit();
			}
		}
	}
}