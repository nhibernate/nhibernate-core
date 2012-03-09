﻿using System;
using System.Collections.Generic;
using System.Linq;
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
			Assert.AreEqual(3, query.Intersect(new[] { "ayende", "rahien", "nhibernate" })
								   .ToList().Count);
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
			Assert.Pass();
		}

		[Test]
		[Ignore("Broken, please fix. See NH-2707")]
		public void CanProjectCollections()
		{
			var query = db.Orders.Select(o => o.OrderLines);
			var result = query.ToList();
			Assert.Pass();
		}

		[Test]
		[Ignore("Broken, please fix. See NH-2707")]
		public void CanProjectCollectionsInsideAnonymousType()
		{
			var query = db.Orders.Select(o => new { o.OrderId, o.OrderLines });
			var result = query.ToList();
			Assert.Pass();
		}

		[Test]
		public void CanProjectComplexDictionaryIndexer()
		{
			//NH-3000
			var lookup = new[] { 1, 2, 3, 4 }.ToDictionary(x => x, x => new { Codes = new[] { x } });
			var query = from item in db.Users
						select new
						{
							index = Array.IndexOf(lookup[item.Id].Codes, item.Id, 0) / 7,
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
							index = lookup[item.Id],
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
							index = lookup[item.Id],
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
							index = lookup.ContainsKey(item.Id),
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
							index = lookup.Contains(item.Id),
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
							index = lookup.Contains(item.Id),
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
									   Value = value.Substring(item.Id),
								   };

			var result = query.ToList()
				.Select(x => x.Value)
				.OrderBy(x => x)
				.ToList();

			Assert.That(result.Count, Is.EqualTo(3));
			Assert.That(result[0], Is.EqualTo("234567890"));
			Assert.That(result[1], Is.EqualTo("34567890"));
			Assert.That(result[2], Is.EqualTo("4567890"));
		}

		private string FormatName(string name, DateTime? lastLoginDate)
		{
			return string.Format("User {0} logged in at {1}", name, lastLoginDate);
		}
	}
}
