using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class ProjectionsTests : LinqTestCase
	{
		[Test]
		public void ProjectAnonymousTypeWithWhere()
		{
			var query = (from user in db.Users
						 where user.Name == "ayende"
						 select user.Name)
				.First();
			Assert.AreEqual("ayende", query);
		}

		[Test]
		public void ProjectConditionals()
		{
			var query = (from user in db.Users
						 orderby user.Id
						 select new { user.Id, GreaterThan2 = user.Id > 2 ? "Yes" : "No" })
				.ToList();
			Assert.AreEqual("No", query[0].GreaterThan2);
			Assert.AreEqual("No", query[1].GreaterThan2);
			Assert.AreEqual("Yes", query[2].GreaterThan2);
		}

		[Test]
		public void ProjectAnonymousTypeWithMultiply()
		{
			var query = (from user in db.Users
						 select new { user.Name, user.Id, Id2 = user.Id * 2 })
				.ToList();
			Assert.AreEqual(3, query.Count);
			foreach (var user in query)
			{
				Assert.AreEqual(user.Id * 2, user.Id2);
			}
		}

		[Test]
		public void ProjectAnonymousTypeWithSubstraction()
		{
			var query = (from user in db.Users
						 select new { user.Name, user.Id, Id2 = user.Id - 2 })
				.ToList();
			Assert.AreEqual(3, query.Count);
			foreach (var user in query)
			{
				Assert.AreEqual(user.Id - 2, user.Id2);
			}
		}

		[Test]
		public void ProjectAnonymousTypeWithDivision()
		{
			var query = (from user in db.Users
						 select new { user.Name, user.Id, Id2 = (user.Id * 10) / 2 })
				.ToList();
			Assert.AreEqual(3, query.Count);
			foreach (var user in query)
			{
				Assert.AreEqual((user.Id * 10) / 2, user.Id2);
			}
		}

		[Test]
		public void ProjectAnonymousTypeWithAddition()
		{
			var query = (from user in db.Users
						 select new { user.Name, user.Id, Id2 = (user.Id + 101) })
				.ToList();
			Assert.AreEqual(3, query.Count);
			foreach (var user in query)
			{
				Assert.AreEqual((user.Id + 101), user.Id2);
			}
		}

		[Test]
		public void ProjectAnonymousTypeAndConcatenateFields()
		{
			var query = (from user in db.Users
						 orderby user.Name
						 select new { DoubleName = user.Name + " " + user.Name, user.RegisteredAt }

						)
				.ToList();

			Assert.AreEqual("ayende ayende", query[0].DoubleName);
			Assert.AreEqual("nhibernate nhibernate", query[1].DoubleName);
			Assert.AreEqual("rahien rahien", query[2].DoubleName);


			Assert.AreEqual(new DateTime(2010, 06, 17), query[0].RegisteredAt);
			Assert.AreEqual(new DateTime(2000, 1, 1), query[1].RegisteredAt);
			Assert.AreEqual(new DateTime(1998, 12, 31), query[2].RegisteredAt);
		}

		[Test]
		public void ProjectKnownType()
		{
			var query = (from user in db.Users
						 orderby user.Id
						 select new KeyValuePair<string, DateTime>(user.Name, user.RegisteredAt))
				.ToList();

			Assert.AreEqual("ayende", query[0].Key);
			Assert.AreEqual("rahien", query[1].Key);
			Assert.AreEqual("nhibernate", query[2].Key);


			Assert.AreEqual(new DateTime(2010, 06, 17), query[0].Value);
			Assert.AreEqual(new DateTime(1998, 12, 31), query[1].Value);
			Assert.AreEqual(new DateTime(2000, 1, 1), query[2].Value);
		}

		[Test]
		public void ProjectAnonymousType()
		{
			var query = (from user in db.Users
						 orderby user.Id
						 select new { user.Name, user.RegisteredAt })
				.ToList();
			Assert.AreEqual("ayende", query[0].Name);
			Assert.AreEqual("rahien", query[1].Name);
			Assert.AreEqual("nhibernate", query[2].Name);


			Assert.AreEqual(new DateTime(2010, 06, 17), query[0].RegisteredAt);
			Assert.AreEqual(new DateTime(1998, 12, 31), query[1].RegisteredAt);
			Assert.AreEqual(new DateTime(2000, 1, 1), query[2].RegisteredAt);
		}

		[Test]
		public void ProjectUserNames()
		{
			var query = (from user in db.Users
						 select user.Name).ToList();
			Assert.AreEqual(3, query.Count);
			Assert.AreEqual(3, query.Intersect(new[] { "ayende", "rahien", "nhibernate" }).Count());
		}

		[Test]
		public void CanCallLocalMethodsInSelect()
		{
			var query = (
							from user in db.Users
							orderby user.Id
							select FormatName(user.Name, user.LastLoginDate)
						).ToList();

			Assert.AreEqual(3, query.Count);
			Assert.IsTrue(query[0].StartsWith("User ayende logged in at"));
			Assert.IsTrue(query[1].StartsWith("User rahien logged in at"));
			Assert.IsTrue(query[2].StartsWith("User nhibernate logged in at"));
		}

		[Test]
		public void CanCallLocalMethodsInAnonymousTypeInSelect()
		{
			var query = (
							from user in db.Users
							orderby user.Id
							select new { Title = FormatName(user.Name, user.LastLoginDate) }
						).ToList();

			Assert.AreEqual(3, query.Count);
			Assert.IsTrue(query[0].Title.StartsWith("User ayende logged in at"));
			Assert.IsTrue(query[1].Title.StartsWith("User rahien logged in at"));
			Assert.IsTrue(query[2].Title.StartsWith("User nhibernate logged in at"));
		}

		[Test]
		public void CanPerformStringOperationsInSelect()
		{
			var query = (
							from user in db.Users
							orderby user.Id
							select new { Title = "User " + user.Name + " logged in at " + user.LastLoginDate }
						).ToList();

			Assert.AreEqual(3, query.Count);
			Assert.IsTrue(query[0].Title.StartsWith("User ayende logged in at"));
			Assert.IsTrue(query[1].Title.StartsWith("User rahien logged in at"));
			Assert.IsTrue(query[2].Title.StartsWith("User nhibernate logged in at"));
		}

		[Test]
		public void CanUseConstantStringInProjection()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							Category = "something"
						};

			var firstUser = query.First();
			Assert.IsNotNull(firstUser);
			firstUser.Category.Should().Be("something");
		}

		[Test]
		public void CanProjectManyCollections()
		{
			var query = db.Orders.SelectMany(o => o.OrderLines);
			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(2155));
		}

		[Test]
		public void CanProjectCollections()
		{
			var query = db.Orders.Select(o => o.OrderLines);
			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
		}

		[Test]
		public void CanProjectCollectionsInsideAnonymousType()
		{
			var query = db.Orders.Select(o => new { o.OrderId, o.OrderLines });
			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
		}

		[Test]
		public void ProjectAnonymousTypeWithCollection()
		{
			// NH-3333
			// done by WCF DS: context.Orders.Expand(o => o.OrderLines) from the client 
			var query = from o in db.Orders
						select new { o, o.OrderLines };

			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
			Assert.That(result[0].o.OrderLines, Is.EquivalentTo(result[0].OrderLines));
		}

		[Test]
		public void ProjectAnonymousTypeWithCollection1()
		{
			// NH-3333
			// done by WCF DS: context.Orders.Expand(o => o.OrderLines) from the client 
			var query = from o in db.Orders
						select new { o.OrderLines, o };

			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
			Assert.That(result[0].o.OrderLines, Is.EquivalentTo(result[0].OrderLines));
		}

		[Test]
		public void ProjectAnonymousTypeWithCollection2()
		{
			// NH-3333
			// done by WCF DS: context.Orders.Expand(o => o.OrderLines) from the client 
			var query = from o in db.Orders
						select new { o.OrderLines, A = 1, B = 2 };

			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
		}

		[Test]
		public void ProjectAnonymousTypeWithCollection3()
		{
			// NH-3333
			// done by WCF DS: context.Orders.Expand(o => o.OrderLines) from the client 
			var query = from o in db.Orders
						select new { OrderLines = o.OrderLines.ToList() };

			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
		}

		[Test]
		public void ProjectKnownTypeWithCollection()
		{
			var query = from o in db.Orders
						select new ExpandedWrapper<Order, ISet<OrderLine>>
							{
								ExpandedElement = o,
								ProjectedProperty0 = o.OrderLines,
								Description = "OrderLine",
								ReferenceDescription = "OrderLine"
							};

			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
			Assert.That(result[0].ExpandedElement.OrderLines, Is.EquivalentTo(result[0].ProjectedProperty0));
		}
		
		[Test]
		public void ProjectKnownTypeWithCollection2()
		{
			var query = from o in db.Orders
						select new ExpandedWrapper<Order, IEnumerable<OrderLine>>
							{
								ExpandedElement = o,
								ProjectedProperty0 = o.OrderLines.Select(x => x),
								Description = "OrderLine",
								ReferenceDescription = "OrderLine"
							};

			var result = query.ToList();
			Assert.That(result.Count, Is.EqualTo(830));
			Assert.That(result[0].ExpandedElement.OrderLines, Is.EquivalentTo(result[0].ProjectedProperty0));
		}

		[Test]
		public void ProjectNestedKnownTypeWithCollection()
		{
			var query = from o in db.Products
				select new ExpandedWrapper<Product, ExpandedWrapper<Supplier, IEnumerable<Product>>>
				{
					ExpandedElement = o,
					ProjectedProperty0 = new ExpandedWrapper<Supplier, IEnumerable<Product>>
					{
						ExpandedElement = o.Supplier,
						ProjectedProperty0 = o.Supplier.Products,
						Description = "Products",
						ReferenceDescription = ""
					},
					Description = "Supplier",
					ReferenceDescription = "Supplier"
				};

			var result = query.ToList();
			Assert.That(result, Has.Count.EqualTo(77));
			Assert.That(result[0].ExpandedElement.Supplier, Is.EqualTo(result[0].ProjectedProperty0.ExpandedElement));
			Assert.That(result[0].ExpandedElement.Supplier.Products,
				Is.EquivalentTo(result[0].ProjectedProperty0.ProjectedProperty0));
		}

		[Test]
		public void ProjectNestedAnonymousTypeWithCollection()
		{
			var query = from o in db.Products
				select new
				{
					ExpandedElement = o,
					ProjectedProperty0 = new
					{
						ExpandedElement = o.Supplier,
						ProjectedProperty0 = o.Supplier.Products,
						Description = "Products",
						ReferenceDescription = ""
					},
					Description = "Supplier",
					ReferenceDescription = "Supplier"
				};

			var result = query.ToList();
			Assert.That(result, Has.Count.EqualTo(77));
			Assert.That(result[0].ExpandedElement.Supplier, Is.EqualTo(result[0].ProjectedProperty0.ExpandedElement));
			Assert.That(result[0].ExpandedElement.Supplier.Products,
				Is.EquivalentTo(result[0].ProjectedProperty0.ProjectedProperty0));
		}

		[Test]
		public void ProjectNestedAnonymousTypeWithProjectedCollection()
		{
			var query = from o in db.Products
				select new
				{
					ExpandedElement = o,
					ProjectedProperty0 = new
					{
						ExpandedElement = o.Supplier,
						ProjectedProperty0 = o.Supplier.Products.Select(x => new {x.Name}),
						Description = "Products",
						ReferenceDescription = ""
					},
					Description = "Supplier",
					ReferenceDescription = "Supplier"
				};

			var result = query.ToList();
			Assert.That(result, Has.Count.EqualTo(77));
			Assert.That(result.Single(x => x.ExpandedElement.ProductId == 1).ProjectedProperty0.ProjectedProperty0.Count(),
				Is.EqualTo(3));
		}

		[Test]
		public void CanProjectComplexDictionaryIndexer()
		{
			//NH-3000
			var lookup = new[] { 1, 2, 3, 4 }.ToDictionary(x => x, x => new { Codes = new[] { x } });
			var query = from item in db.Users
						select new
						{
							index = Array.IndexOf(lookup[1 + item.Id % 4].Codes, 1 + item.Id % 4, 0) / 7,
						};

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
		}

		[Test]
		public void CanProjectComplexParameterDictionaryIndexer()
		{
			//NH-3000
			var lookup = new[] { 1, 2, 3, 4 }.ToDictionary(x => x, x => new { Codes = new[] { x } });
			var query = from item in db.Users
						select new
						{
							index = lookup[1 + item.Id % 4],
						};

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
		}

		[Test]
		public void CanProjectParameterDictionaryIndexer()
		{
			//NH-3000
			var lookup = new[] { 1, 2, 3, 4 }.ToDictionary(x => x, x => x);
			var query = from item in db.Users
						select new
						{
							index = lookup[1 + item.Id % 4],
						};

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
		}

		[Test]
		public void CanProjectParameterDictionaryContainsKey()
		{
			//NH-3000
			var lookup = new[] { 1, 2, 3, 4 }.ToDictionary(x => x, x => x);
			var query = from item in db.Users
						select new
						{
							isPresent  = lookup.ContainsKey(item.Id),
						};

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
		}

		[Test]
		public void CanProjectParameterArrayContains()
		{
			//NH-3000
			var lookup = new[] { 1, 2, 3, 4 };
			var query = from item in db.Users
						select new
						{
							isPresent = lookup.Contains(item.Id),
						};

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
		}

		[Test]
		public void CanProjectParameterStringContains()
		{
			//NH-3000
			var lookup = new[] { 1, 2, 3, 4 };
			var query = from item in db.Users
						select new
						{
							isPresent = lookup.Contains(item.Id),
						};

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
		}

		[Test]
		public void CanProjectParameterSubstring()
		{
			//NH-3000
			const string value = "1234567890";

			var query = from item in db.Users
						select new
								   {
									   Start = item.Id % 10,
									   Value = value.Substring(item.Id % 10),
								   };

			var result = query.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
			Assert.That(result[0].Value, Is.EqualTo(value.Substring(result[0].Start)));
			Assert.That(result[1].Value, Is.EqualTo(value.Substring(result[1].Start)));
			Assert.That(result[2].Value, Is.EqualTo(value.Substring(result[2].Start)));
		}

		private string FormatName(string name, DateTime? lastLoginDate)
		{
			return string.Format("User {0} logged in at {1}", name, lastLoginDate);
		}


		/// <summary>
		/// This mimic classes in System.Data.Services.Internal.
		/// </summary>
		class ExpandedWrapper<TExpandedElement>
		{
			public TExpandedElement ExpandedElement { get; set; }
			public string Description { get; set; }
			public string ReferenceDescription { get; set; }
		}

		/// <summary>
		/// This mimic classes in System.Data.Services.Internal.
		/// </summary>
		class ExpandedWrapper<TExpandedElement, TProperty0> : ExpandedWrapper<TExpandedElement>
		{
			public TProperty0 ProjectedProperty0 { get; set; }
		}
	}
}
