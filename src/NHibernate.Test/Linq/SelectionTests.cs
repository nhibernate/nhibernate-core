﻿using System;
using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class SelectionTests : LinqTestCase
	{
		[Test]
		public void CanGetCountOnQueryWithAnonymousType()
		{
			var query = from user in db.Users
						select new { user.Name, RoleName = user.Role.Name };

			int totalCount = query.Count();

			Assert.AreEqual(3, totalCount);
		}

		[Test]
		public void CanGetFirstWithAnonymousType()
		{
			var query = from user in db.Users
						select new { user.Name, RoleName = user.Role.Name };

			var firstUser = query.First();

			Assert.IsNotNull(firstUser);
		}

		[Test]
		public void CanAggregateWithAnonymousType()
		{
			var query = from user in db.Users
						select new { user.Name, RoleName = user.Role.Name };

			var userInfo = query.Aggregate((u1, u2) => u1);

			Assert.IsNotNull(userInfo);
		}

		[Test]
		public void CanSelectUsingMemberInitExpression()
		{
			var query = from user in db.Users
						select new UserDto(user.Id, user.Name) { InvalidLoginAttempts = user.InvalidLoginAttempts };

			var list = query.ToList();
			Assert.AreEqual(3, list.Count);
		}

		[Test]
		public void CanSelectNestedAnonymousType()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							Enums = new
							{
								user.Enum1,
								user.Enum2
							},
							RoleName = user.Role.Name
						};

			var list = query.ToList();
			Assert.AreEqual(3, list.Count);

			//assert role names -- to ensure that the correct values were used to invoke constructor
			Assert.IsTrue(list.All(u => u.RoleName == "Admin" || u.RoleName == "User" || String.IsNullOrEmpty(u.RoleName)));
		}

		[Test]
		public void CanSelectNestedAnonymousTypeWithMultipleReferences()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							Enums = new
							{
								user.Enum1,
								user.Enum2
							},
							RoleName = user.Role.Name,
							RoleIsActive = (bool?) user.Role.IsActive
						};

			var list = query.ToList();
			Assert.AreEqual(3, list.Count);

			//assert role names -- to ensure that the correct values were used to invoke constructor
			Assert.IsTrue(list.All(u => u.RoleName == "Admin" || u.RoleName == "User" || String.IsNullOrEmpty(u.RoleName)));
		}

		[Test]
		public void CanSelectNestedAnonymousTypeWithComponentReference()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							Enums = new
							{
								user.Enum1,
								user.Enum2
							},
							RoleName = user.Role.Name,
							ComponentProperty = user.Component.Property1
						};

			var list = query.ToList();
			Assert.AreEqual(3, list.Count);

			//assert role names -- to ensure that the correct values were used to invoke constructor
			Assert.IsTrue(list.All(u => u.RoleName == "Admin" || u.RoleName == "User" || String.IsNullOrEmpty(u.RoleName)));
		}

		[Test]
		public void CanSelectNestedMemberInitExpression()
		{
			var query = from user in db.Users
						select new UserDto(user.Id, user.Name)
						{
							InvalidLoginAttempts = user.InvalidLoginAttempts,
							Dto2 = new UserDto2
									   {
								RegisteredAt = user.RegisteredAt,
								Enum = user.Enum2
							},
							RoleName = user.Role.Name
						};

			var list = query.ToList();
			Assert.AreEqual(3, list.Count);

			//assert role names -- to ensure that the correct values were used to invoke constructor
			Assert.IsTrue(list.All(u => u.RoleName == "Admin" || u.RoleName == "User" || String.IsNullOrEmpty(u.RoleName)));
		}

		[Test]
		public void CanSelectNestedMemberInitWithinNewExpression()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							user.InvalidLoginAttempts,
							Dto = new UserDto2
									  {
								RegisteredAt = user.RegisteredAt,
								Enum = user.Enum2
							},
							RoleName = user.Role.Name
						};

			var list = query.ToList();
			Assert.AreEqual(3, list.Count);

			//assert role names -- to ensure that the correct values were used to invoke constructor
			Assert.IsTrue(list.All(u => u.RoleName == "Admin" || u.RoleName == "User" || String.IsNullOrEmpty(u.RoleName)));
		}

		[Test]
		public void CanSelectSingleProperty()
		{
			var query = from user in db.Users
						where user.Name == "ayende"
						select user.RegisteredAt;

			DateTime date = query.Single();
			Assert.AreEqual(new DateTime(2010, 06, 17), date);
		}

		[Test]
		public void CanSelectWithProxyInterface()
		{
			var query = (from user in db.IUsers
						 where user.Name == "ayende"
						 select user).ToArray();

			Assert.AreEqual(1, query.Length);
			Assert.AreEqual("ayende", query.First().Name);
		}

		[Test]
		public void CanSelectBinaryExpressions()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							IsSmall = (user.Enum1 == EnumStoredAsString.Small)
						};

			var list = query.ToList();

			foreach (var user in list)
			{
				if (user.Name == "rahien")
				{
					Assert.IsTrue(user.IsSmall);
				}
				else
				{
					Assert.IsFalse(user.IsSmall);
				}
			}
		}

		[Test]
		public void CanSelectWithMultipleBinaryExpressions()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							IsAyende = (user.Enum1 == EnumStoredAsString.Medium
								&& user.Enum2 == EnumStoredAsInt32.High)
						};

			var list = query.ToList();

			foreach (var user in list)
			{
				if (user.Name == "ayende")
				{
					Assert.IsTrue(user.IsAyende);
				}
				else
				{
					Assert.IsFalse(user.IsAyende);
				}
			}
		}

		[Test]
		public void CanSelectWithMultipleBinaryExpressionsWithOr()
		{
			var query = from user in db.Users
						select new
						{
							user.Name,
							IsAyende = (user.Name == "ayende"
								|| user.Name == "rahien")
						};

			var list = query.ToList();

			foreach (var user in list)
			{
				if (user.Name == "ayende" || user.Name == "rahien")
				{
					Assert.IsTrue(user.IsAyende);
				}
				else
				{
					Assert.IsFalse(user.IsAyende);
				}
			}
		}

		[Test]
		public void CanSelectWithAnySubQuery()
		{
			var query = from timesheet in db.Timesheets
						select new
						{
							timesheet.Id,
							HasEntries = timesheet.Entries.Any()
						};

			var list = query.ToList();

			Assert.AreEqual(2, list.Count(t => t.HasEntries));
			Assert.AreEqual(1, list.Count(t => !t.HasEntries));
		}

		[Test]
		public void CanSelectWithAggregateSubQuery()
		{
			var timesheets = (from timesheet in db.Timesheets
							  select new
							  {
								  timesheet.Id,
								  EntryCount = timesheet.Entries.Count
							  }).ToArray();

			Assert.AreEqual(3, timesheets.Length);
			Assert.AreEqual(0, timesheets[0].EntryCount);
			Assert.AreEqual(2, timesheets[1].EntryCount);
			Assert.AreEqual(4, timesheets[2].EntryCount);
		}


		[Test]
		public void CanSelectWrappedType()
		{
			//NH-2151
			var query = from user in db.Users
						select new Wrapper<User> { item = user, message = user.Name + " " + user.Role };

			Assert.IsTrue(query.ToArray().Length > 0);
		}

		[Test]
		public void CanDoProjectionWithCast()
		{
			// NH-2463
			var lst1 = db.Users.Select(p => new { p1 = p.Name }).ToList();

			Assert.AreEqual(3, lst1.Count());

			var lst2 = db.Users.Select(p => new { p1 = (p as User).Name }).ToList();

			Assert.AreEqual(3, lst2.Count());
		}

		public class Wrapper<T>
		{
			public T item;
			public string message;
		}
	}
}
