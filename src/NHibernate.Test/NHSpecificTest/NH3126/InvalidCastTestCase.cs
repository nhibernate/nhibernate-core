using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH3126
{
	[TestFixture]
	public class InvalidCastWithGenericDictionaryOnCascadeTest : BugTestCase
	{
		[Test]
		public void Test()
		{
			var property = new Property { Name = "Property 1" };
			using (var session = OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					session.Save(property);

					tx.Commit();
				}

				var item = new Item();
				using (var tx = session.BeginTransaction())
				{
					item.Name = "Item 1";
					item.PropertyValues = new Dictionary<Guid, PropertyValue>
											  {
												  {property.Id, new PropertyValue {Value = "Value 1"}}
											  };

					session.Save(item);

					tx.Commit();
				}
				session.Clear();

				var savedItem = session.Get<Item>(item.Id);
				Assert.AreEqual(1, savedItem.PropertyValues.Count);
				Assert.AreEqual("Value 1", savedItem.PropertyValues[property.Id].Value);
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (var session = OpenSession())
			{
				session.Delete("from System.Object");
				session.Flush();
			}
		}
	}
}