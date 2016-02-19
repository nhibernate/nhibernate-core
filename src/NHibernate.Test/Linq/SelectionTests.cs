using System;
using System.Collections.Generic;
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

		[Test, KnownBug("NH-3045")]
		public void CanSelectFirstElementFromChildCollection()
		{
			using (var log = new SqlLogSpy())
			{
				var orders = db.Customers
					.Select(customer => customer.Orders.OrderByDescending(x => x.OrderDate).First())
					.ToList();

				Assert.That(orders, Has.Count.GreaterThan(0));

				var text = log.GetWholeLog();
				var count = text.Split(new[] { "SELECT" }, StringSplitOptions.None).Length - 1;
				Assert.That(count, Is.EqualTo(1));
			}
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
		public void CanProjectWithCast()
		{
			// NH-2463
			// ReSharper disable RedundantCast

			var names1 = db.Users.Select(p => new { p1 = p.Name }).ToList();
			Assert.AreEqual(3, names1.Count);

			var names2 = db.Users.Select(p => new { p1 = ((User) p).Name }).ToList();
			Assert.AreEqual(3, names2.Count);

			var names3 = db.Users.Select(p => new { p1 = (p as User).Name }).ToList();
			Assert.AreEqual(3, names3.Count);

			var names4 = db.Users.Select(p => new { p1 = ((IUser) p).Name }).ToList();
			Assert.AreEqual(3, names4.Count);

			var names5 = db.Users.Select(p => new { p1 = (p as IUser).Name }).ToList();
			Assert.AreEqual(3, names5.Count);
			// ReSharper restore RedundantCast
		}

		[Test]
		public void CanSelectAfterOrderByAndTake()
		{
			// NH-3320
			var names = db.Users.OrderBy(p => p.Name).Take(3).Select(p => p.Name).ToList();
			Assert.AreEqual(3, names.Count);
		}

		[Test]
		public void CanSelectManyWithCast()
		{
			// NH-2688
			// ReSharper disable RedundantCast
			var orders1 = db.Customers.Where(c => c.CustomerId == "VINET").SelectMany(o => o.Orders).ToList();
			Assert.AreEqual(5, orders1.Count);

			//$exception	{"c.Orders is not mapped [.SelectMany[NHibernate.DomainModel.Northwind.Entities.Customer,NHibernate.DomainModel.Northwind.Entities.Order](.Where[NHibernate.DomainModel.Northwind.Entities.Customer](NHibernate.Linq.NhQueryable`1[NHibernate.DomainModel.Northwind.Entities.Customer], Quote((c, ) => (String.op_Equality(c.CustomerId, p1))), ), Quote((o, ) => (Convert(o.Orders))), )]"}	System.Exception {NHibernate.Hql.Ast.ANTLR.QuerySyntaxException} 
			// Block OData navigation to detail request requests like 
			// http://localhost:2711/TestWcfDataService.svc/TestEntities(guid&#39;0dd52f6c-1943-4013-a88e-3b63a1fbe11b&#39;)/Details1 
			var orders2 = db.Customers.Where(c => c.CustomerId == "VINET").SelectMany(o => (ISet<Order>) o.Orders).ToList();
			Assert.AreEqual(5, orders2.Count);

			//$exception	{"([100001].Orders As ISet`1)"}	System.Exception {System.NotSupportedException} 
			var orders3 = db.Customers.Where(c => c.CustomerId == "VINET").SelectMany(o => (o.Orders as ISet<Order>)).ToList();
			Assert.AreEqual(5, orders3.Count);

			var orders4 = db.Customers.Where(c => c.CustomerId == "VINET").SelectMany(o => (IEnumerable<Order>) o.Orders).ToList();
			Assert.AreEqual(5, orders4.Count);

			var orders5 = db.Customers.Where(c => c.CustomerId == "VINET").SelectMany(o => (o.Orders as IEnumerable<Order>)).ToList();
			Assert.AreEqual(5, orders5.Count);
			// ReSharper restore RedundantCast
		}

		[Test]
		public void CanSelectCollection()
		{
			var orders = db.Customers.Where(c => c.CustomerId == "VINET").Select(o => o.Orders).ToList();
			Assert.AreEqual(5, orders[0].Count);
		}

		[Test]
		public void CanSelectConditionalKnownTypes()
		{
     		var moreThanTwoOrderLinesBool = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? true : false }).ToList();
			Assert.That(moreThanTwoOrderLinesBool.Count(x => x.HasMoreThanTwo == true), Is.EqualTo(410));

			var moreThanTwoOrderLinesNBool = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? true : (bool?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNBool.Count(x => x.HasMoreThanTwo == true), Is.EqualTo(410));

			var moreThanTwoOrderLinesShort = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? (short)1 : (short)0 }).ToList();
			Assert.That(moreThanTwoOrderLinesShort.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesNShort = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? (short?)1 : (short?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNShort.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesInt = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1 : 0 }).ToList();
			Assert.That(moreThanTwoOrderLinesInt.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesNInt = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1 : (int?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNInt.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesDecimal = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1m : 0m }).ToList();
			Assert.That(moreThanTwoOrderLinesDecimal.Count(x => x.HasMoreThanTwo == 1m), Is.EqualTo(410));

			var moreThanTwoOrderLinesNDecimal = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1m : (decimal?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNDecimal.Count(x => x.HasMoreThanTwo == 1m), Is.EqualTo(410));

			var moreThanTwoOrderLinesSingle = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1f : 0f }).ToList();
			Assert.That(moreThanTwoOrderLinesSingle.Count(x => x.HasMoreThanTwo == 1f), Is.EqualTo(410));

			var moreThanTwoOrderLinesNSingle = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1f : (float?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNSingle.Count(x => x.HasMoreThanTwo == 1f), Is.EqualTo(410));

			var moreThanTwoOrderLinesDouble = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1d : 0d }).ToList();
			Assert.That(moreThanTwoOrderLinesDouble.Count(x => x.HasMoreThanTwo == 1d), Is.EqualTo(410));

			var moreThanTwoOrderLinesNDouble = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1d : (double?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNDouble.Count(x => x.HasMoreThanTwo == 1d), Is.EqualTo(410));
			
			var moreThanTwoOrderLinesString = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? "yes" : "no" }).ToList();
			Assert.That(moreThanTwoOrderLinesString.Count(x => x.HasMoreThanTwo == "yes"), Is.EqualTo(410));

			var now = DateTime.Now.Date;
			var moreThanTwoOrderLinesDateTime = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.OrderDate.Value : now }).ToList();
			Assert.That(moreThanTwoOrderLinesDateTime.Count(x => x.HasMoreThanTwo != now), Is.EqualTo(410));

			var moreThanTwoOrderLinesNDateTime = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.OrderDate : null }).ToList();
			Assert.That(moreThanTwoOrderLinesNDateTime.Count(x => x.HasMoreThanTwo != null), Is.EqualTo(410));

			var moreThanTwoOrderLinesGuid = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.Shipper.Reference : Guid.Empty }).ToList();
			Assert.That(moreThanTwoOrderLinesGuid.Count(x => x.HasMoreThanTwo != Guid.Empty), Is.EqualTo(410));

			var moreThanTwoOrderLinesNGuid = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.Shipper.Reference : (Guid?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNGuid.Count(x => x.HasMoreThanTwo != null), Is.EqualTo(410));
		}

		[Test]
		public void CanSelectConditionalEntity()
		{
			var fatherInsteadOfChild = db.Animals.Select(a => a.Father.SerialNumber == "5678" ? a.Father : a).ToList();
			Assert.That(fatherInsteadOfChild, Has.Exactly(2).With.Property("SerialNumber").EqualTo("5678"));
		}

		[Test]
		public void CanSelectConditionalObject()
		{
			var fatherIsKnown = db.Animals.Select(a => new { a.SerialNumber, Superior = a.Father.SerialNumber, FatherIsKnown = a.Father.SerialNumber == "5678" ? (object)true : (object)false }).ToList();
			Assert.That(fatherIsKnown, Has.Exactly(1).With.Property("FatherIsKnown").True);
		}

		public class Wrapper<T>
		{
			public T item;
			public string message;
		}
	}
}
