using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH473
{
	[TestFixture]
	public class Fixture:BugTestCase
	{
		protected override void OnSetUp()
		{
			using(var session=this.OpenSession())
			using(var tran=session.BeginTransaction())
			{
				var parent = new Parent();
				var child1 = new Child {Name = "Fabio"};
				var child2 = new Child {Name = "Ayende"};
				var child3 = new Child {Name = "Dario"};
				parent.Children.Add(child1);
				parent.Children.Add(child2);
				parent.Children.Add(child3);
				session.Save(parent);
				tran.Commit();
			}
		}
		protected override void OnTearDown()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				session.Delete("from Parent");
				tran.Commit();
			}
		}
		[Test]
		public void ChildItemsGetInOrderWhenUsingFetchJoin()
		{
			using(var session=this.OpenSession())
			using(var tran=session.BeginTransaction())
			{
				var result = session.CreateCriteria(typeof (Parent))
					.SetFetchMode("Children", FetchMode.Join)
					.List<Parent>();
				Assert.That(result[0].Children[0].Name,Is.EqualTo("Ayende"));
				Assert.That(result[0].Children[1].Name, Is.EqualTo("Dario"));
				Assert.That(result[0].Children[2].Name, Is.EqualTo("Fabio"));
			}
		}

		[Test]
		public void ChildItemsGetInOrderWhenUsingFetchJoinUniqueResult()
		{
			using (var session = this.OpenSession())
			using (var tran = session.BeginTransaction())
			{
				var result = session.CreateCriteria(typeof(Parent))
					.SetFetchMode("Children", FetchMode.Join)
					.UniqueResult<Parent>();
				Assert.That(result.Children[0].Name, Is.EqualTo("Ayende"));
				Assert.That(result.Children[1].Name, Is.EqualTo("Dario"));
				Assert.That(result.Children[2].Name, Is.EqualTo("Fabio"));
			}
		}
	}
}
