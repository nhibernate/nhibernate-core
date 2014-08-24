using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Driver;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3142
{
	[TestFixture]
	public class ChildrenTest : BugTestCase
	{
		protected override bool AppliesTo(Engine.ISessionFactoryImplementor factory)
		{
			return !(factory.ConnectionProvider.Driver is OracleManagedDataClientDriver);
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (var session = OpenSession())
			{
				for (int h = 1; h < 3; h++)
				{
					for (int i = 1; i < 4; i++)
					{
						var parent = new DomainParent { Id1 = h, Id2 = i };
						session.Save(parent);

						var parentId = new DomainParentWithComponentId { Id = { Id1 = h, Id2 = i } };
						session.Save(parentId);

						for (int j = 1; j < 4; j++)
						{
							var child = new DomainChild { ParentId1 = h, ParentId2 = i };
							session.Save(child);

							var childId = new DomainChildWCId { ParentId1 = h, ParentId2 = i };
							session.Save(childId);
						}
					}
				}

				session.Flush();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				session.Delete("from DomainChild");
				session.Delete("from DomainChildWCId");
				session.Delete("from System.Object");
				session.Flush();
			}
		}


		[Test]
		public void ChildrenCollectionOfAllParentsShouldContainsThreeElements()
		{
			using (var session = OpenSession())
			{
				var entities = session.CreateQuery("from DomainParent").List<DomainParent>();

				foreach (var parent in entities)
					Assert.AreEqual(3, parent.Children.Count);
			}
		}

		[Test]
		public void ChildrenCollectionOfAllParentsWithComponentIdShouldContainsThreeElements()
		{
			using (var session = OpenSession())
			{
				var entities = session.CreateQuery("from DomainParentWithComponentId").List<DomainParentWithComponentId>();

				foreach (var parent in entities)
					Assert.AreEqual(3, parent.Children.Count);
			}
		}
	}
}
