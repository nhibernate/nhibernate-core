using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.DomainModel.NHSpecific;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Driver;
using NHibernate.Exceptions;
using NHibernate.Proxy;
using NHibernate.Type;
using NUnit.Framework;
using static NHibernate.Linq.ExpressionEvaluation;

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
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore(Dialect.GetType().Name + " does not support scalar sub-queries");

			var timesheets = (from timesheet in db.Timesheets orderby timesheet.Id
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
		public void CanSelectConditional()
		{
			// SqlServerCeDriver and OdbcDriver have an issue matching the case statements inside select and order by statement,
			// when having one or more parameters inside them. Throws with the following error:
			// ORDER BY items must appear in the select list if SELECT DISTINCT is specified.
			if (!(Sfi.ConnectionProvider.Driver is OdbcDriver) && !(Sfi.ConnectionProvider.Driver is SqlServerCeDriver))
			{
				using (var sqlLog = new SqlLogSpy())
				{
					var q = db.Orders.Where(o => o.Customer.CustomerId == "test")
							   .Select(o => o.ShippedTo.Contains("test") ? o.ShippedTo : o.Customer.CompanyName)
							   .OrderBy(o => o)
							   .Distinct()
							   .ToList();

					Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "case"), Is.EqualTo(2));
				}
			}

			using (var sqlLog = new SqlLogSpy())
			{
				var q = db.Orders.Where(o => o.Customer.CustomerId == "test")
						   .Select(o => o.OrderDate.HasValue ? o.OrderDate : o.ShippingDate)
						   .FirstOrDefault();

				Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "case"), Is.EqualTo(1));
			}

			using (var sqlLog = new SqlLogSpy())
			{
				var q = db.Orders.Where(o => o.Customer.CustomerId == "test")
						   .Select(o => new
						   {
							   Value = o.OrderDate.HasValue
								   ? o.Customer.CompanyName
								   : (o.ShippingDate.HasValue
									? o.Shipper.CompanyName + "Shipper"
									: o.ShippedTo)
						   })
						   .FirstOrDefault();

				var log = sqlLog.GetWholeLog();
				Assert.That(FindAllOccurrences(log, "as col"), Is.EqualTo(1));
			}

			using (var sqlLog = new SqlLogSpy())
			{
				var q = db.Orders.Where(o => o.Customer.CustomerId == "test")
				          .Select(o => new
				          {
					          Value = o.OrderDate.HasValue
						          ? o.Customer.CompanyName
						          : (o.ShippingDate.HasValue
							          ? o.Shipper.CompanyName + "Shipper"
									  : null)
				          })
				          .FirstOrDefault();

				var log = sqlLog.GetWholeLog();
				Assert.That(FindAllOccurrences(log, "as col"), Is.EqualTo(1));
			}

			using (var sqlLog = new SqlLogSpy())
			{
				var q = db.Orders.Where(o => o.Customer.CustomerId == "test")
						  .Select(o => new
						  {
							  Value = o.OrderDate.HasValue
								  ? o.Customer.CompanyName
								  : (o.ShippingDate.HasValue
									  ? o.Shipper.CompanyName + "Shipper"
									  : "default")
						  })
						  .FirstOrDefault();

				var log = sqlLog.GetWholeLog();
				Assert.That(FindAllOccurrences(log, "as col"), Is.EqualTo(1));
			}

			var defaultValue = "default";
			using (var sqlLog = new SqlLogSpy())
			{
				var q = db.Orders.Where(o => o.Customer.CustomerId == "test")
						  .Select(o => new
						  {
							  Value = o.OrderDate.HasValue
								  ? o.Customer.CompanyName
								  : (o.ShippingDate.HasValue
									  ? o.Shipper.CompanyName + "Shipper"
									  : defaultValue)
						  })
						  .FirstOrDefault();

				var log = sqlLog.GetWholeLog();
				Assert.That(FindAllOccurrences(log, "as col"), Is.EqualTo(1));
			}
		}

		[Test]
		public void CanSelectConditionalSubQuery()
		{
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore(Dialect.GetType().Name + " does not support scalar sub-queries");

			var list = db.Customers
						   .Select(c => new
						   {
							   Date = db.Orders.Where(o => o.Customer.CustomerId == c.CustomerId)
										.Select(o => o.OrderDate.HasValue ? o.OrderDate : o.ShippingDate)
										.Max()
						   })
						   .ToList();
			Assert.That(list, Has.Count.GreaterThan(0));

			var list2 = db.Orders
			              .Select(
				              o => new
				              {
					              UnitPrice = o.Freight.HasValue
						              ? o.OrderLines.Where(l => l.Discount == 1)
						                 .Select(l => l.Product.UnitPrice.HasValue ? l.Product.UnitPrice : l.UnitPrice)
						                 .Max()
						              : o.OrderLines.Where(l => l.Discount == 0)
						                 .Select(l => l.Product.UnitPrice.HasValue ? l.Product.UnitPrice : l.UnitPrice)
						                 .Max()
				              })
			              .ToList();
			Assert.That(list2, Has.Count.GreaterThan(0));

			var list3 = db.Orders
						  .Select(o => new
						  {
							  Date = o.OrderLines.Any(l => o.OrderDate.HasValue)
								  ? db.Employees
									  .Select(e => e.BirthDate.HasValue ? e.BirthDate : e.HireDate)
									  .Max()
								  : o.Employee.Superior != null ? o.Employee.Superior.BirthDate : o.Employee.BirthDate
						  })
						  .ToList();
			Assert.That(list3, Has.Count.GreaterThan(0));

			var list4 = db.Orders
						  .Select(o => new
						  {
							  Employee = db.Employees.Any(e => e.Superior != null)
								  ? db.Employees
									  .Where(e => e.Superior != null)
									  .Select(e => e.Superior).FirstOrDefault()
								  : o.Employee.Superior != null ? o.Employee.Superior : o.Employee
						  })
						  .ToList();
			Assert.That(list4, Has.Count.GreaterThan(0));
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
		public void CanSelectNotMappedEntityProperty()
		{
			var list = db.Animals.Where(o => o.Mother != null).Select(o => o.FatherOrMother.SerialNumber).ToList();

			Assert.That(list, Has.Count.GreaterThan(0));
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

			var names6 = db.Users.Select(p => new { p1 = (long) p.Id }).ToList();
			Assert.AreEqual(3, names6.Count);

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
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore(Dialect.GetType().Name + " does not support scalar sub-queries");

			var moreThanTwoOrderLinesBool = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? true : false, Param = true }).ToList();
			Assert.That(moreThanTwoOrderLinesBool.Count(x => x.HasMoreThanTwo == true), Is.EqualTo(410));

			var moreThanTwoOrderLinesNBool = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? true : (bool?)null, Param = (bool?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNBool.Count(x => x.HasMoreThanTwo == true), Is.EqualTo(410));

			var moreThanTwoOrderLinesShort = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? (short)1 : (short)0, Param = (short)0 }).ToList();
			Assert.That(moreThanTwoOrderLinesShort.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesNShort = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? (short?)1 : (short?)null, Param = (short?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNShort.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesInt = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1 : 0, Param = 1 }).ToList();
			Assert.That(moreThanTwoOrderLinesInt.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesNInt = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1 : (int?)null, Param = (int?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNInt.Count(x => x.HasMoreThanTwo == 1), Is.EqualTo(410));

			var moreThanTwoOrderLinesDecimal = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1m : 0m, Param = 1m }).ToList();
			Assert.That(moreThanTwoOrderLinesDecimal.Count(x => x.HasMoreThanTwo == 1m), Is.EqualTo(410));

			var moreThanTwoOrderLinesNDecimal = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1m : (decimal?)null, Param = (decimal?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNDecimal.Count(x => x.HasMoreThanTwo == 1m), Is.EqualTo(410));

			var moreThanTwoOrderLinesSingle = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1f : 0f, Param = 1f }).ToList();
			Assert.That(moreThanTwoOrderLinesSingle.Count(x => x.HasMoreThanTwo == 1f), Is.EqualTo(410));

			var moreThanTwoOrderLinesNSingle = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1f : (float?)null, Param = (float?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNSingle.Count(x => x.HasMoreThanTwo == 1f), Is.EqualTo(410));

			var moreThanTwoOrderLinesDouble = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1d : 0d, Param = 1d }).ToList();
			Assert.That(moreThanTwoOrderLinesDouble.Count(x => x.HasMoreThanTwo == 1d), Is.EqualTo(410));

			var moreThanTwoOrderLinesNDouble = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? 1d : (double?)null, Param = (double?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNDouble.Count(x => x.HasMoreThanTwo == 1d), Is.EqualTo(410));
			
			var moreThanTwoOrderLinesString = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? "yes" : "no", Param = "no" }).ToList();
			Assert.That(moreThanTwoOrderLinesString.Count(x => x.HasMoreThanTwo == "yes"), Is.EqualTo(410));

			var now = DateTime.Now.Date;
			var moreThanTwoOrderLinesDateTime = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.OrderDate.Value : now, Param = now }).ToList();
			Assert.That(moreThanTwoOrderLinesDateTime.Count(x => x.HasMoreThanTwo != now), Is.EqualTo(410));

			var moreThanTwoOrderLinesNDateTime = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.OrderDate : null, Param = (DateTime?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNDateTime.Count(x => x.HasMoreThanTwo != null), Is.EqualTo(410));

			var moreThanTwoOrderLinesGuid = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.Shipper.Reference : Guid.Empty, Param = Guid.Empty }).ToList();
			Assert.That(moreThanTwoOrderLinesGuid.Count(x => x.HasMoreThanTwo != Guid.Empty), Is.EqualTo(410));

			var moreThanTwoOrderLinesNGuid = db.Orders.Select(o => new { Id = o.OrderId, HasMoreThanTwo = o.OrderLines.Count() > 2 ? o.Shipper.Reference : (Guid?)null, Param = (Guid?)null }).ToList();
			Assert.That(moreThanTwoOrderLinesNGuid.Count(x => x.HasMoreThanTwo != null), Is.EqualTo(410));
		}

		[Test]
		public void CanSelectConditionalEntity()
		{
			var fatherInsteadOfChild = db.Animals.Select(a => a.Father.SerialNumber == "5678" ? a.Father : a).ToList();
			Assert.That(fatherInsteadOfChild, Has.Exactly(2).With.Property("SerialNumber").EqualTo("5678"));
		}

		[Test]
		public void CanSelectConditionalEntityWithCast()
		{
			var fatherInsteadOfChild = db.Mammals.Select(a => a.Father.SerialNumber == "5678" ? (object)a.Father : (object)a).ToList();
			Assert.That(fatherInsteadOfChild, Has.Exactly(2).With.Property("SerialNumber").EqualTo("5678"));
		}

		[Test]
		public void CanSelectConditionalEntityValue()
		{
			var fatherInsteadOfChild = db.Animals.Select(a => a.Father.SerialNumber == "5678" ? a.Father.SerialNumber : a.SerialNumber).ToList();
			Assert.That(fatherInsteadOfChild, Has.Exactly(2).EqualTo("5678"));
		}

		[Test]
		public void CanSelectConditionalEntityValueWithEntityComparison()
		{
			var father = db.Animals.Single(a => a.SerialNumber == "5678");
			var fatherInsteadOfChild = db.Animals.Select(a => a.Father == father ? a.Father.SerialNumber : a.SerialNumber).ToList();
			Assert.That(fatherInsteadOfChild, Has.Exactly(2).EqualTo("5678"));
		}

		[Test]
		public void CanSelectModulus()
		{
			var list = db.Animals.Select(a => new { Sql = a.Id % 2.1f, a.Id }).ToList();
			Assert.That(list.Select(o => o.Sql), Is.EqualTo(list.Select(o => o.Id % 2.1f)).Within(GetTolerance()));
			var list1 = db.Animals.Select(a => new { Sql = a.Id % 2.1d, a.Id }).ToList();
			Assert.That(list1.Select(o => o.Sql), Is.EqualTo(list1.Select(o => o.Id % 2.1d)).Within(GetTolerance()));
			var list2 = db.Animals.Select(a => new { Sql = a.BodyWeight % 2.1f, a.BodyWeight }).ToList();
			Assert.That(list2.Select(o => o.Sql), Is.EqualTo(list2.Select(o => o.BodyWeight % 2.1f)).Within(GetTolerance()));
			var list3 = db.Animals.Select(a => new { Sql = a.Id % 2.1m, a.Id }).ToList();
			Assert.That(list3.Select(o => o.Sql), Is.EqualTo(list3.Select(o => o.Id % 2.1m)));
			var list4 = db.Animals.Select(a => new { Sql = a.Id % 2, a.Id }).ToList();
			Assert.That(list4.Select(o => o.Sql), Is.EqualTo(list4.Select(o => o.Id % 2)));
			var list5 = db.Animals.Select(a => new { Sql = a.Id % 2L, a.Id }).ToList();
			Assert.That(list5.Select(o => o.Sql), Is.EqualTo(list5.Select(o => o.Id % 2L)));
			var list7 = db.Animals.Select(a => new { Sql = a.BodyWeight % 2, a.BodyWeight }).ToList();
			Assert.That(list7.Select(o => o.Sql), Is.EqualTo(list7.Select(o => o.BodyWeight % 2)));
			var list8 = db.Animals.Select(a => new { Sql = a.BodyWeight % 2L, a.BodyWeight }).ToList();
			Assert.That(list8.Select(o => o.Sql), Is.EqualTo(list8.Select(o => o.BodyWeight % 2L)));
			var list9 = db.Products.Select(a => new { Sql = a.UnitPrice % 2L, a.UnitPrice }).ToList();
			Assert.That(list9.Select(o => o.Sql), Is.EqualTo(list9.Select(o => o.UnitPrice % 2L)));
			var list10 = db.Products.Select(a => new { Sql = a.UnitPrice % 2, a.UnitPrice }).ToList();
			Assert.That(list10.Select(o => o.Sql), Is.EqualTo(list10.Select(o => o.UnitPrice % 2)));
		}

		[Test]
		public void CanSelectModulusSameExpression()
		{
			var list1 = db.Animals.Select(a => new ObjectDto { CalculatedValue = a.Id % 2.1m, OriginalValue = a.Id }).ToList();
			Assert.That(list1.Select(o => o.CalculatedValue), Is.EqualTo(list1.Select(o => o.OriginalValue % 2.1m)));
			var list2 = db.Animals.Select(a => new ObjectDto { CalculatedValue = a.Id % 2L, OriginalValue = a.Id }).ToList();
			Assert.That(list2.Select(o => o.CalculatedValue), Is.EqualTo(list2.Select(o => o.OriginalValue % 2L)));
			var list3 = db.Animals.Select(a => new ObjectDto { CalculatedValue = a.Id % 2.1f, OriginalValue = a.Id }).ToList();
			Assert.That(list3.Select(o => o.CalculatedValue), Is.EqualTo(list3.Select(o => o.OriginalValue % 2.1f)).Within(GetTolerance()));
			var list4 = db.Animals.Select(a => new ObjectDto { CalculatedValue = a.Id % 2.1d, OriginalValue = a.Id }).ToList();
			Assert.That(list4.Select(o => o.CalculatedValue), Is.EqualTo(list4.Select(o => o.OriginalValue % 2.1d)).Within(GetTolerance()));
		}

		[Test]
		public void CanForceDatabaseEvaluation()
		{
			var namedParameters = !(Sfi.ConnectionProvider.Driver is OdbcDriver);
			Assert.That(GetSqlSelect(db.Animals.Select(a => DatabaseEval(() => 5))), Does.Contain(namedParameters ? "p0" : "?"));
			Assert.That(GetSqlSelect(db.Products.Select(a => DatabaseEval(() => a.UnitPrice * 1234.4321m))), Does.Contain("*"));
			Assert.That(FindAllOccurrences(GetSqlSelect(db.Products.Select(a => new
			{
				Server = DatabaseEval(() => a.UnitPrice * 1234.4321m),
				Default = a.UnitPrice * 1234.4321m
			})), "*"), Is.EqualTo(1));
		}

		[Test]
		public void CanForceClientEvaluation()
		{
			var query = db.Animals.Select(a => ClientEval(() => a.Id + 5));
			Assert.That(GetSqlSelect(query), Does.Not.Contain("+"));
			Assert.That(query.ToList(), Is.EqualTo(db.Animals.Select(a => a.Id + 5).ToList()));

			query = db.Animals.Select(a => ClientEval(() => a.SerialNumber.Length));
			Assert.That(GetSqlSelect(query), Does.Not.Contain("len(").And.Not.Contain("length("));
			Assert.That(query.ToList(), Is.EqualTo(db.Animals.Select(a => a.SerialNumber.Length).ToList()));

			var query2 = db.Animals.Select(a => ClientEval(() => a.SerialNumber.Substring(0, 1)));
			Assert.That(GetSqlSelect(query2), Does.Not.Contain("substr(").And.Not.Contain("substring("));
			Assert.That(query2.ToList(), Is.EqualTo(db.Animals.Select(a => a.SerialNumber.Substring(0, 1)).ToList()));

			query2 = db.Animals.Select(a => ClientEval(() => a.Id % 2 == 0 ? a.SerialNumber : a.Description));
			Assert.That(GetSqlSelect(query2), Does.Not.Contain("case"));
			Assert.That(query2.ToList(), Is.EqualTo(db.Animals.Select(a => a.Id % 2 == 0 ? a.SerialNumber : a.Description).ToList()));

			var query3 = db.Animals.Select(a => new
			{
				Client = ClientEval(() => a.Id % 2 == 0 ? a.SerialNumber.Substring(0, 1) : a.Description),
				Server = a.Id % 2 == 0 ? a.SerialNumber.Substring(0, 1) : a.Description,
			}).ToList();
			Assert.That(query3.Select(o => o.Client), Is.EqualTo(query3.Select(o => o.Server)));
		}

		[Test]
		public void CanSelectMultiplyOperator()
		{
			var list1 = db.Animals.Select(a => new { Sql = a.Id * 5, a.Id }).ToList();
			Assert.That(list1.Select(o => o.Sql), Is.EqualTo(list1.Select(o => o.Id * 5)));
			var list2 = db.Animals.Select(a => new { Sql = a.Id * 12345.54321m, a.Id }).ToList();
			Assert.That(list2.Select(o => o.Sql), Is.EqualTo(list2.Select(o => o.Id * 12345.54321m)));
			var list3 = db.Animals.Select(a => new { Sql = a.Id * 123.321f, a.Id }).ToList();
			Assert.That(list3.Select(o => o.Sql), Is.EqualTo(list3.Select(o => o.Id * 123.321f)).Within(GetTolerance()));
			var list4 = db.Animals.Select(a => new { Sql = a.Id * 12345.54321d, a.Id }).ToList();
			Assert.That(list4.Select(o => o.Sql), Is.EqualTo(list4.Select(o => o.Id * 12345.54321d)).Within(GetTolerance()));
			var list5 = db.Animals.Select(a => new { Sql = a.Id * 2L, a.Id }).ToList();
			Assert.That(list5.Select(o => o.Sql), Is.EqualTo(list5.Select(o => o.Id * 2L)));

			var list6 = db.Products.Select(a => new { Sql = a.UnitPrice * 12345.54321m, a.UnitPrice }).ToList();
			Assert.That(list6.Select(o => o.Sql), Is.EqualTo(list6.Select(o => o.UnitPrice * 12345.54321m)));
			var list7 = db.Products.Select(a => new { Sql = a.UnitPrice * 12345L, a.UnitPrice }).ToList();
			Assert.That(list7.Select(o => o.Sql), Is.EqualTo(list7.Select(o => o.UnitPrice * 12345L)));

			var list8 = db.Animals.Select(a => new { Sql = a.BodyWeight * 12345.54321f, a.BodyWeight }).ToList();
			Assert.That(list8.Select(o => o.Sql), Is.EqualTo(list8.Select(o => o.BodyWeight * 12345.54321f)));
		}

		[Test]
		public void CanSelectDivideOperator()
		{
			var list1 = db.Animals.Select(a => new { Sql = a.Id / 5, a.Id }).ToList();
			Assert.That(list1.Select(o => o.Sql), Is.EqualTo(list1.Select(o => o.Id / 5)));
			var list2 = db.Animals.Select(a => new { Sql = a.Id / 12345.54321m, a.Id }).ToList();
			Assert.That(list2.Select(o => o.Sql), Is.EqualTo(list2.Select(o => o.Id / 12345.54321m)));
			var list3 = db.Animals.Select(a => new { Sql = a.Id / 12345.54321f, a.Id }).ToList();
			Assert.That(list3.Select(o => o.Sql), Is.EqualTo(list3.Select(o => o.Id / 12345.54321f)).Within(GetTolerance()));
			var list4 = db.Animals.Select(a => new { Sql = a.Id / 12345.54321d, a.Id }).ToList();
			Assert.That(list4.Select(o => o.Sql), Is.EqualTo(list4.Select(o => o.Id / 12345.54321d)).Within(GetTolerance()));
			var list5 = db.Animals.Select(a => new { Sql = a.Id / 2L, a.Id }).ToList();
			Assert.That(list5.Select(o => o.Sql), Is.EqualTo(list5.Select(o => o.Id / 2L)));

			var list6 = db.Products.Select(a => new { Sql = a.UnitPrice / 12345.54321m, a.UnitPrice }).ToList();
			Assert.That(list6.Select(o => o.Sql), Is.EqualTo(list6.Select(o => o.UnitPrice / 12345.54321m)));
			var list7 = db.Products.Select(a => new { Sql = a.UnitPrice.Value / 12345L, a.UnitPrice }).ToList();
			Assert.That(list7.Select(o => o.Sql), Is.EqualTo(list7.Select(o => o.UnitPrice / 12345L)));

			var list8 = db.Animals.Select(a => new { Sql = a.BodyWeight / 12345.54321f, a.BodyWeight }).ToList();
			Assert.That(list8.Select(o => o.Sql), Is.EqualTo(list8.Select(o => o.BodyWeight / 12345.54321f)).Within(GetTolerance()));
		}

		[Test]
		public void CanSelectAddOperator()
		{
			var list1 = db.Animals.Select(a => new { Sql = a.Id + 5, a.Id }).ToList();
			Assert.That(list1.Select(o => o.Sql), Is.EqualTo(list1.Select(o => o.Id + 5)));
			var list2 = db.Animals.Select(a => new { Sql = a.Id + 12345.54321m, a.Id }).ToList();
			Assert.That(list2.Select(o => o.Sql), Is.EqualTo(list2.Select(o => o.Id + 12345.54321m)));
			var list3 = db.Animals.Select(a => new { Sql = a.Id + 12345.54321f, a.Id }).ToList();
			Assert.That(list3.Select(o => o.Sql), Is.EqualTo(list3.Select(o => o.Id + 12345.54321f)).Within(GetTolerance()));
			var list4 = db.Animals.Select(a => new { Sql = a.Id + 12345.54321d, a.Id }).ToList();
			Assert.That(list4.Select(o => o.Sql), Is.EqualTo(list4.Select(o => o.Id + 12345.54321d)));
			var list5 = db.Animals.Select(a => new { Sql = a.Id + 2L, a.Id }).ToList();
			Assert.That(list5.Select(o => o.Sql), Is.EqualTo(list5.Select(o => o.Id + 2L)));

			var list6 = db.Products.Select(a => new { Sql = a.UnitPrice + 12345.54321m, a.UnitPrice }).ToList();
			Assert.That(list6.Select(o => o.Sql), Is.EqualTo(list6.Select(o => o.UnitPrice + 12345.54321m)));
			var list7 = db.Products.Select(a => new { Sql = a.UnitPrice + 12345L, a.UnitPrice }).ToList();
			Assert.That(list7.Select(o => o.Sql), Is.EqualTo(list7.Select(o => o.UnitPrice + 12345L)));

			var list8 = db.Animals.Select(a => new { Sql = a.BodyWeight + 12345.54321f, a.BodyWeight }).ToList();
			Assert.That(list8.Select(o => o.Sql), Is.EqualTo(list8.Select(o => o.BodyWeight + 12345.54321f)));
		}

		[Test]
		public void CanSelectSubtractOperator()
		{
			var list1 = db.Animals.Select(a => new { Sql = a.Id - 5, a.Id }).ToList();
			Assert.That(list1.Select(o => o.Sql), Is.EqualTo(list1.Select(o => o.Id - 5)));
			var list2 = db.Animals.Select(a => new { Sql = a.Id - 12345.54321m, a.Id }).ToList();
			Assert.That(list2.Select(o => o.Sql), Is.EqualTo(list2.Select(o => o.Id - 12345.54321m)));
			var list3 = db.Animals.Select(a => new { Sql = a.Id - 12345.54321f, a.Id }).ToList();
			Assert.That(list3.Select(o => o.Sql), Is.EqualTo(list3.Select(o => o.Id - 12345.54321f)).Within(GetTolerance()));
			var list4 = db.Animals.Select(a => new { Sql = a.Id - 12345.54321d, a.Id }).ToList();
			Assert.That(list4.Select(o => o.Sql), Is.EqualTo(list4.Select(o => o.Id - 12345.54321d)));
			var list5 = db.Animals.Select(a => new { Sql = a.Id - 2L, a.Id }).ToList();
			Assert.That(list5.Select(o => o.Sql), Is.EqualTo(list5.Select(o => o.Id - 2L)));

			var list6 = db.Products.Select(a => new { Sql = a.UnitPrice - 12345.54321m, a.UnitPrice }).ToList();
			Assert.That(list6.Select(o => o.Sql), Is.EqualTo(list6.Select(o => o.UnitPrice - 12345.54321m)));
			var list7 = db.Products.Select(a => new { Sql = a.UnitPrice - 12345L, a.UnitPrice }).ToList();
			Assert.That(list7.Select(o => o.Sql), Is.EqualTo(list7.Select(o => o.UnitPrice - 12345L)));

			var list8 = db.Animals.Select(a => new { Sql = a.BodyWeight - 12345.54321f, a.BodyWeight }).ToList();
			Assert.That(list8.Select(o => o.Sql), Is.EqualTo(list8.Select(o => o.BodyWeight - 12345.54321f)));
		}

		private class ObjectDto
		{
			public object CalculatedValue { get; set; }

			public int OriginalValue { get; set; }
		}

		[Test]
		public void CanSelectConditionalEntityValueWithEntityComparisonComplex()
		{
			var animal = db.Animals.Select(
				               a => new
				               {
								   Parent = a.Father != null || a.Mother != null ? (a.Father ?? a.Mother) : null,
								   ParentSerialNumber = a.Father != null || a.Mother != null ? (a.Father ?? a.Mother).SerialNumber : null,
								   Parent2 = a.Mother ?? a.Father,
								   a.Father,
								   a.Mother
				               })
			               .FirstOrDefault(o => o.ParentSerialNumber == "5678");

			Assert.That(animal, Is.Not.Null);
			Assert.That(animal.Father, Is.Not.Null);
			Assert.That(animal.Mother, Is.Not.Null);
			Assert.That(animal.Parent, Is.Not.Null);
			Assert.That(animal.Parent2, Is.Not.Null);
			Assert.That(NHibernateUtil.IsInitialized(animal.Parent), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(animal.Parent2), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(animal.Father), Is.True);
			Assert.That(NHibernateUtil.IsInitialized(animal.Mother), Is.True);
		}

		[Test]
		public void CanSelectConditionalEntityValueWithEntityCast()
		{
			var list = db.Animals.Select(
							   a => new
							   {
								   BodyWeight = (double?) (a is Cat
									   ? (a.Father ?? a.Mother).BodyWeight
										: (a is Dog
											? (a.Mother ?? a.Father).BodyWeight
											: (a.Father.Father.BodyWeight)
									   ))
							   })
						   .ToList();
			Assert.That(list, Has.Exactly(1).With.Property("BodyWeight").Not.Null);
		}

		[Test]
		public void CanSelectBinaryClientSideTest()
		{
			var exception = Assert.Throws<GenericADOException>(() =>
			{
				db.Animals.Select(a => a.FatherOrMother.BodyWeight + a.BodyWeight).ToList();
			});
			Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
			Assert.That(exception.InnerException.Message, Is.EqualTo(
				"Null value cannot be assigned to a value type 'System.Double'. Cast expression '([a].FatherOrMother.BodyWeight + [a].BodyWeight)' to 'System.Nullable`1[System.Double]'."));

			var list = db.Animals.Select(a => (double?) (a.FatherOrMother.BodyWeight + a.BodyWeight)).ToList();
			Assert.That(list, Has.Exactly(5).Null.And.Exactly(1).EqualTo(271d));

			// Arithmetic operator
			var list2 = db.Animals.Select(a => new
			{
				// Left side null
				Client = (double?) (a.FatherOrMother.BodyWeight + a.BodyWeight + a.Father.BodyWeight),
				Server = (double?) (a.Father ?? a.Mother).BodyWeight + a.BodyWeight + a.Father.BodyWeight,
				// Right side null
				Client2 = (double?) (a.BodyWeight - a.Father.BodyWeight - a.FatherOrMother.BodyWeight),
				Server2 = (double?) a.BodyWeight - a.Father.BodyWeight - (a.Father ?? a.Mother).BodyWeight
			}).ToList();
			Assert.That(list2.Select(o => o.Client), Is.EqualTo(list2.Select(o => o.Server)));
			Assert.That(list2.Select(o => o.Client2), Is.EqualTo(list2.Select(o => o.Server2)));

			// Boolean logic operator
			var list3 = db.Users.Select(u => new
			{
				// Left side null
				Client = u.NotMappedUser.Role.IsActive && true,
				Server = u.Role.IsActive && true,
				// Right side null
				Client2 = true && u.NotMappedUser.Role.IsActive,
				Server2 = true && u.Role.IsActive
			}).ToList();
			Assert.That(list3.Select(o => o.Client), Is.EqualTo(list3.Select(o => o.Server)));
			Assert.That(list3.Select(o => o.Client2), Is.EqualTo(list3.Select(o => o.Server2)));

			list3 = db.Users.Select(u => new
			{
				// Left side null
				Client = u.NotMappedUser.Role.IsActive || true,
				Server = u.Role.IsActive || true,
				// Right side null
				Client2 = false || u.NotMappedUser.Role.IsActive,
				Server2 = false || u.Role.IsActive
			}).ToList();
			Assert.That(list3.Select(o => o.Client), Is.EqualTo(list3.Select(o => o.Server)));
			Assert.That(list3.Select(o => o.Client2), Is.EqualTo(list3.Select(o => o.Server2)));

			// Comparison operator
			list3 = db.Users.Select(u => new
			{
				// Left side null
				Client = u.NotMappedUser.Role.Id > 0,
				Server = u.Role.Id > 0,
				// Right side null
				Client2 = 0 < u.NotMappedUser.Role.Id,
				Server2 = 0 < u.Role.Id
			}).ToList();
			Assert.That(list3.Select(o => o.Client), Is.EqualTo(list3.Select(o => o.Server)));
			Assert.That(list3.Select(o => o.Client2), Is.EqualTo(list3.Select(o => o.Server2)));

			// Bitwise boolean operator
			var list4 = db.Users.Select(u => new
			{
				// Left side null
				Client = (bool?) (u.NotMappedUser.Role.IsActive | true),
				Server = (bool?) (u.Role.IsActive | true),
				// Right side null
				Client2 = (bool?) (true | u.NotMappedUser.Role.IsActive),
				Server2 = (bool?) (true | u.Role.IsActive)
			}).ToList();
			Assert.That(list4.Select(o => o.Client), Is.EqualTo(list4.Select(o => o.Server)));
			Assert.That(list4.Select(o => o.Client2), Is.EqualTo(list4.Select(o => o.Server2)));

			// Bitwise number operator
			var list5 = db.Users.Select(u => new
			{
				// Left side null
				Client = (int?) (u.NotMappedUser.Role.Id | 5),
				Server = (int?) (u.Role.Id | 5),
				// Right side null
				Client2 = (int?) (5 | u.NotMappedUser.Role.Id),
				Server2 = (int?) (5 | u.Role.Id)
			}).ToList();
			Assert.That(list5.Select(o => o.Client), Is.EqualTo(list5.Select(o => o.Server)));
			Assert.That(list5.Select(o => o.Client2), Is.EqualTo(list5.Select(o => o.Server2)));

			// Coalesce operator
			var list6 = db.Users.Select(u => new
			{
				// Left side null
				Client = u.NotMappedUser.Role.Name ?? u.NotMappedUser.Name,
				Server = u.Role.Name ?? u.Name,
				// Right side null
				Client2 = u.NotMappedUser.Name ?? u.NotMappedUser.Role.Name,
				Server2 = u.Name ?? u.Role.Name,
				// Both side null
				Client3 = u.NotMappedUser.Role.Name ?? u.NotMappedUser.Role.Name,
				Server3 = u.Role.Name ?? u.Role.Name
			}).ToList();
			Assert.That(list6.Select(o => o.Client), Is.EqualTo(list6.Select(o => o.Server)));
			Assert.That(list6.Select(o => o.Client2), Is.EqualTo(list6.Select(o => o.Server2)));
			Assert.That(list6.Select(o => o.Client3), Is.EqualTo(list6.Select(o => o.Server3)));
		}

		[Test]
		public void CanSelectUnaryClientSideTest()
		{
			var exception = Assert.Throws<GenericADOException>(() =>
			{
				db.Animals.Select(a => -a.FatherOrMother.BodyWeight).ToList();
			});
			Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
			Assert.That(exception.InnerException.Message, Is.EqualTo(
				"Null value cannot be assigned to a value type 'System.Double'. Cast expression '-[a].FatherOrMother.BodyWeight' to 'System.Nullable`1[System.Double]'."));

			// Negate
			var list = db.Animals.Select(a => new
			{
				Client = (double?) -a.FatherOrMother.BodyWeight,
				Server = (double?) -((a.Father ?? a.Mother).BodyWeight)
			}).ToList();
			Assert.That(list.Select(o => o.Client), Is.EqualTo(list.Select(o => o.Server)));

			// Convert
			list = db.Animals.Select(a => new
			{
				Client = (double?) a.FatherOrMother.BodyWeight,
				Server = (double?) (a.Father ?? a.Mother).BodyWeight
			}).ToList();
			Assert.That(list.Select(o => o.Client), Is.EqualTo(list.Select(o => o.Server)));

			// UnaryPlus
			list = db.Animals.Select(a => new
			{
				Client = (double?) +a.FatherOrMother.BodyWeight,
				Server = (double?) +((a.Father ?? a.Mother).BodyWeight)
			}).ToList();
			Assert.That(list.Select(o => o.Client), Is.EqualTo(list.Select(o => o.Server)));

			// Not
			var list2 = db.Users.Select(u => new
			{
				Client = (bool?) !u.NotMappedUser.Role.IsActive,
				Server = (bool?) !u.Role.IsActive
			}).ToList();
			Assert.That(list2.Select(o => o.Client), Is.EqualTo(list2.Select(o => o.Server)));

			// Convert value type
			var list3 = db.Users.Select(u => (int?) (u.Role != null ? 5 : 10)).ToList();
			Assert.That(list3, Has.Exactly(3).Not.Null);

			// Convert enum
			list3 = db.Users.Select(u => (int?) u.Role.CreatedBy.Enum2).ToList();
			Assert.That(list3, Has.Exactly(3).Null);

			// Convert reference type
			var list4 = db.Animals.Select(a => new
			{ 
				Client = (Dog) a.FatherOrMother,
				Server = (Dog) (a.Father ?? a.Mother)
			}).ToList();
			Assert.That(list4.Select(o => o.Client), Is.EqualTo(list4.Select(o => o.Server)));

			// TypeAs
			list4 = db.Animals.Select(a => new
			{
				Client = a.FatherOrMother as Dog,
				Server = (a.Father ?? a.Mother) as Dog
			}).ToList();
			Assert.That(list4.Select(o => o.Client), Is.EqualTo(list4.Select(o => o.Server)));

			// Convert constant reference type
			var list5 = db.Animals.Select(a => (Animal) new Dog()).ToList();
			Assert.That(list5, Has.Exactly(6).Not.Null);
		}

		[Test]
		public void CanSelectConditionalClientSideWithNullValueTypeTest()
		{
			var exception = Assert.Throws<GenericADOException>(() =>
			{
				db.Animals.Select(
							   a => new
							   {
								   BodyWeight = (string.IsNullOrWhiteSpace(a.Description)
										? a.Mother.Mother.BodyWeight
										: a.Father.Mother.BodyWeight)
							   })
						   .ToList();
			});
			Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
			Assert.That(exception.InnerException.Message, Is.EqualTo(
				"Null value cannot be assigned to a value type 'System.Double'. " +
				"Cast expression 'IIF(IsNullOrWhiteSpace([a].Description), [_3].BodyWeight, [_1].BodyWeight)' to 'System.Nullable`1[System.Double]'."));

			var list = db.Animals.Select(
							   a => new
							   {
								   BodyWeight = (double?) (string.IsNullOrWhiteSpace(a.Description)
										? a.Mother.Mother.BodyWeight
										: a.Father.Mother.BodyWeight)
							   })
						   .ToList();
			Assert.That(list, Has.Exactly(0).With.Property("BodyWeight").Not.Null);

			var list2 = db.Animals.Select(
							   a => new
							   {
								   BodyWeight = (double?) (string.IsNullOrWhiteSpace(a.Description)
										? a.Mother.Mother.BodyWeight
										: 5d)
							   })
						   .ToList();
			Assert.That(list2, Has.Exactly(0).With.Property("BodyWeight").Not.Null);

			var list3 = db.Animals.Select(
							   a => new
							   {
								   BodyWeight = (double?) (string.IsNullOrWhiteSpace(a.Description)
										? 5d
										: a.Father.Mother.BodyWeight)
							   })
						   .ToList();
			Assert.That(list3, Has.Exactly(6).With.Property("BodyWeight").Not.Null);

			var list4 = db.Animals.Select(
							   a => new
							   {
								   BodyWeightHashCode = (int?) ((string.IsNullOrWhiteSpace(a.Description)
										? a.Mother.Mother.BodyWeight
										: a.Father.Mother.BodyWeight)).GetHashCode()
							   })
						   .ToList();
			Assert.That(list4, Has.Exactly(0).With.Property("BodyWeightHashCode").Not.Null);

			var list5 = db.Animals.Select(
							   a => new
							   {
								   BodyWeight = (double?) (string.IsNullOrWhiteSpace(a.Description)
										? (string.IsNullOrWhiteSpace(a.Description)
											? a.Mother.Mother.BodyWeight
											: a.Father.Mother.BodyWeight)
										: (string.IsNullOrWhiteSpace(a.Description)
											? a.Mother.Mother.BodyWeight
											: a.Father.Mother.BodyWeight))
							   })
						   .ToList();
			Assert.That(list5, Has.Exactly(0).With.Property("BodyWeight").Not.Null);

			var list6 = db.Animals.Select(
							   a => new
							   {
								   Client =  a.Father.HasFather ? (double?) null : a.BodyWeight,
								   Server = a.Father.Father != null ? (double?) null : a.BodyWeight,
							   })
						   .ToList();
			Assert.That(list6.Select(o => o.Client), Is.EqualTo(list6.Select(o => o.Server)));

			var list7 = db.Users.Select(
							   a => new
							   {
								   Client = a.NotMappedUser.Role.IsActive ? 1 : 2,
								   Server = a.Role.IsActive ? 1 : 2
							   })
						   .ToList();
			Assert.That(list7.Select(o => o.Client), Is.EqualTo(list7.Select(o => o.Server)));
		}

		[Test]
		public void CanExecuteMethodWithNullObjectClientSideTest()
		{
			var exception = Assert.Throws<GenericADOException>(() =>
			{
				db.Animals.Select(
							  a => new
							  {
								  a.Id,
								  FatherId = a.Father.Father.Id
							  })
						  .ToList();
			});
			Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
			Assert.That(exception.InnerException.Message, Is.EqualTo(
				"Null value cannot be assigned to a value type 'System.Int32'. Cast expression '[_0].Father.Id' to 'System.Nullable`1[System.Int32]'."));

			exception = Assert.Throws<GenericADOException>(() =>
			{
				db.Animals.Select(
							  a => new
							  {
								  a.Id,
								  FatherIdHashCode = a.Father.Father.Id.GetHashCode()
							  })
						  .ToList();
			});
			Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
			Assert.That(exception.InnerException.Message, Is.EqualTo(
				"Null value cannot be assigned to a value type 'System.Int32'. Cast expression '[_1].Id.GetHashCode()' to 'System.Nullable`1[System.Int32]'."));

			var list = db.Animals.Select(
							   a => new
							   {
								   NullableId = (int?) a.Father.Father.Id,
								   NullableIdHashCode = (int?) a.Father.Father.Id.GetHashCode()
							   })
						   .ToList();
			Assert.That(list, Has.Exactly(0).With.Property("NullableId").Not.Null);
		}

		[Test]
		public void CanSelectConstant()
		{
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = a.Id + 1f + 5d }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = a.Id + 1m + 5 }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = 1 }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = "test" }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = 1 + 5 }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Id = a.Id, Test = 1 }));
			AssertOneSelectColumn(db.Animals.Select(a => new { a.Id, Test = "test" }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = 1, Test2 = "test" }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = a.Id, Test2 = new { Value = "test" }, Test3 = 1 }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = new UserDto(1, "test") }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = new UserDto(1, "test"), a.Id }));
			AssertOneSelectColumn(db.Animals.Select(a => new { Test = new UserDto(1, "test") { Dto2 = { Enum = EnumStoredAsInt32.High } }, a.Id }));
			AssertOneSelectColumn(db.Animals.Select(a => new UserDto(1, "test")));
			AssertOneSelectColumn(db.Animals.Select(a => new UserDto(1, "test") {RoleName = a.Description}));
			AssertOneSelectColumn(db.Animals.Select(a => new UserDto(a.Id, "test")));
			AssertOneSelectColumn(db.Animals.Select(a => 1));
			AssertOneSelectColumn(db.Animals.Select(a => "test"));
		}

		[Test]
		public void CanSelectWithIsOperator()
		{
			Assert.DoesNotThrow(() => db.Animals.Select(a => a is Dog).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => a.FatherSerialNumber is string).ToList());
		}

		[Test]
		public void CanSelectComponentProperty()
		{
			AssertOneSelectColumn(db.Users.Select(u => u.Component.Property1));
			AssertOneSelectColumn(db.Users.Select(u => u.Component.OtherComponent.OtherProperty1));
		}

		[Test]
		public void CanSelectNonMappedComponentProperty()
		{
			Assert.DoesNotThrow(() => db.Users.Select(u => u.Component.Property3).ToList());
			Assert.DoesNotThrow(() => db.Users.Select(u => u.Component.OtherComponent.OtherProperty2).ToList());
			var list = db.Users.Select(u => new
			{
				u.Component.OtherComponent.OtherProperty1,
				OtherProperty3 = u.Component.OtherComponent.OtherProperty2,
				u.Component.Property1,
				u.Component.Property2,
				u.Component.Property3
			}).ToList();
			Assert.That(list.Select(o => o.OtherProperty3), Is.EqualTo(list.Select(o => o.OtherProperty1)));
			Assert.That(
				list.Select(o => (o.Property1 ?? o.Property2) == null ? null : $"{o.Property1}{o.Property2}"),
				Is.EqualTo(list.Select(o => o.Property3)));
		}

		[Test]
		public void CanSelectWithAnInvocation()
		{
			Func<string, string> func = s => s + "postfix";
			Assert.DoesNotThrow(() => db.Animals.Select(a => func(a.SerialNumber)).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => func(a.FatherSerialNumber)).ToList());
		}

		[Test]
		public void CanSelectEnumerable()
		{
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new[] { a.Id } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new[] { a.Id, 1 } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new[] { 1 } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new[] { a, a.Father, a.Mother } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new
			{
				Enumerable = new[]
				{
					new UserDto(a.Id, a.FatherSerialNumber) {RoleName = a.FatherSerialNumber},
					new UserDto(1, a.FatherSerialNumber) {RoleName = a.FatherSerialNumber, InvalidLoginAttempts = 1},
					null,
					new UserDto(1, "test") {RoleName = "test", InvalidLoginAttempts = 1},
					new UserDto(1, "test") {Dto2List = {new UserDto2(), new UserDto2()}, Dto2 = {Enum = EnumStoredAsInt32.High}},
					new UserDto(1, a.FatherSerialNumber)
					{
						Dto2List = {new UserDto2() { Enum = a.Id > 0 ? EnumStoredAsInt32.High : EnumStoredAsInt32.Unspecified }, new UserDto2()},
						Dto2 = {Enum = a.Id > 0 ? EnumStoredAsInt32.High : EnumStoredAsInt32.Unspecified}
					}
				}
			}).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new[] { a.SerialNumber, a.FatherSerialNumber, null } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new int[][] { new[] { a.Id }, new[] { 1 }, new[] { a.Id, 1 } } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new List<int> { a.Id, 1 } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new List<int>(5) { a.Id, 1 } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new List<int>(a.Id) { 1 } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new List<string>(a.Id) { a.SerialNumber, a.FatherSerialNumber, null } }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new
			{
				Enumerable = new List<UserDto>(a.Id)
				{
					new UserDto(a.Id, a.FatherSerialNumber) {RoleName = a.FatherSerialNumber},
					new UserDto(1, a.FatherSerialNumber) {RoleName = a.FatherSerialNumber, InvalidLoginAttempts = 1},
					null,
					new UserDto(1, "test") {RoleName = "test", InvalidLoginAttempts = 1},
					new UserDto(1, "test") {Dto2List = {new UserDto2(), new UserDto2()}, Dto2 = {Enum = EnumStoredAsInt32.High}},
					new UserDto(1, a.FatherSerialNumber)
					{
						Dto2List = {new UserDto2() { Enum = a.Id > 0 ? EnumStoredAsInt32.High : EnumStoredAsInt32.Unspecified }, new UserDto2()},
						Dto2 = {Enum = a.Id > 0 ? EnumStoredAsInt32.High : EnumStoredAsInt32.Unspecified}
					}
				}
			}).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new[] { a.SerialNumber, a.FatherSerialNumber, null }[a.Id - a.Id].Length }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new { Enumerable = new List<string> { a.SerialNumber, a.FatherSerialNumber, null }[a.Id - a.Id].Length }).ToList());
			Assert.DoesNotThrow(() => db.Animals.Select(a => new
			{
				Enumerable = new Dictionary<string, string>
			{
				{ a.SerialNumber, a.FatherSerialNumber },
				{ "1", a.Father.SerialNumber },
				{ "2", null }
			}[a.SerialNumber]
			}).ToList());
		}

		[Test]
		public void CanSelectConditionalSubClassPropertyValue()
		{
			var animal = db.Animals.Select(
				               a => new
				               {
					               Pregnant = a is Mammal ? ((Mammal) a).Pregnant : false
				               })
			               .Where(o => o.Pregnant)
			               .ToList();

			Assert.That(animal, Has.Count.EqualTo(1));
		}

		[Test]
		public void CanSelectConditionalEntityValueWithEntityComparisonRepeat()
		{
			// Check again in the same ISessionFactory to ensure caching doesn't cause failures
			CanSelectConditionalEntityValueWithEntityComparison();
		}

		[Test]
		public void CanSelectConditionalObject()
		{
			var fatherIsKnown = db.Animals.Select(a => new { a.SerialNumber, Superior = a.Father.SerialNumber, FatherIsKnown = a.Father.SerialNumber == "5678" ? (object)true : (object)false }).ToList();
			Assert.That(fatherIsKnown, Has.Exactly(1).With.Property("FatherIsKnown").True);
		}

		[Test]
		public void CanCastToDerivedType()
		{
			var dogs = db.Animals
			                      .Where(a => ((Dog) a).Pregnant)
			                      .Select(a => new {a.SerialNumber})
			                      .ToList();
			Assert.That(dogs, Has.Exactly(1).With.Property("SerialNumber").Not.Null);
		}

		[Test]
		public void CanCastToCustomRegisteredType()
		{
			TypeFactory.RegisterType(typeof(NullableInt32), new NullableInt32Type(), Enumerable.Empty<string>());
			Assert.That(db.Users.Where(o => (NullableInt32) o.Id == 1).ToList(), Has.Count.EqualTo(1));
		}

		[Test]
		public void TestClientSideEvaluation()
		{
			var list = db.Animals.Select(a => new
			{
				ClientSide = string.IsNullOrEmpty(a.FatherSerialNumber) ? 1 : 0,
				ClientSide2 = string.IsNullOrEmpty(a.Father.SerialNumber) ? 1 : 0
			}).ToList();
			Assert.That(list.Select(o => o.ClientSide), Is.EqualTo(list.Select(o => o.ClientSide2)));

			var list2 = db.Animals.Select(a => new
			{
				ClientSide = a.Father.IsProxy(),
				ClientSide2 = a.FatherSerialNumber.IsProxy()
			}).ToList();
			Assert.That(list2.Select(o => o.ClientSide), Is.EqualTo(list2.Select(o => o.ClientSide2)));

			var list3 = db.Orders.Where(o => o.OrderDate.HasValue).Select(o => new
			{
				ClientSide = o.OrderDate.Value.TimeOfDay.Days,
				ClientSide2 = o.OrderDate.Value
			}).ToList();
			Assert.That(list3.Select(o => o.ClientSide), Is.EqualTo(list3.Select(o => o.ClientSide2.TimeOfDay.Days)));

			var list4 = db.Orders.Where(o => o.OrderDate.HasValue).Select(o => new
			{
				o.OrderId,
				ClientSide = o.OrderDate.Value.TimeOfDay.CompareTo(new TimeSpan(o.OrderId)),
				ClientSide2 = o.OrderDate.Value
			}).ToList();
			Assert.That(list4.Select(o => o.ClientSide), Is.EqualTo(list4.Select(o => o.ClientSide2.TimeOfDay.CompareTo(new TimeSpan(o.OrderId)))));
		}

		[Test]
		public void TestServerAndClientSideEvaluationComparison()
		{
			var list = db.Animals.Select(
				a => new
				{
					ServerSide = (int?) a.Father.SerialNumber.Length,
					ClientSide = (int?) a.FatherSerialNumber.Length
				}).ToList();
			Assert.That(list.Select(o => o.ClientSide), Is.EqualTo(list.Select(o => o.ServerSide)));

			var list1 = db.Animals
						 .Where(a => a.Father.SerialNumber != null)
						 .Select(
							 a => new
							 {
								 ServerSide = a.Father.SerialNumber.Length,
								 ClientSide = a.FatherSerialNumber.Length
							 })
						 .ToList();
			Assert.That(list1.Select(o => o.ClientSide), Is.EqualTo(list1.Select(o => o.ServerSide)));

			var clientSide = db.Animals.Select(a => a.FatherSerialNumber.Length.ToString()).ToList();
			var serverSide = db.Animals.Select(a => a.FatherSerialNumber.Length.ToString()).ToList();
			Assert.That(clientSide, Is.EqualTo(serverSide));

			var exception = Assert.Throws<GenericADOException>(
				() =>
				{
					db.Animals.Select(
						a => new
						{
							ServerSide = a.Father.SerialNumber.Length
						}).ToList();
				});
			Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
			Assert.That(exception.InnerException.Message, Is.EqualTo(
				"Null value cannot be assigned to a value type 'System.Int32'. Cast expression '[_0].SerialNumber.Length' to 'System.Nullable`1[System.Int32]'."));

			exception = Assert.Throws<GenericADOException>(
				() =>
				{
					db.Animals.Select(
						a => new
						{
							ClientSide = a.FatherSerialNumber.Length
						}).ToList();
				});
			Assert.That(exception.InnerException, Is.TypeOf<InvalidOperationException>());
			Assert.That(exception.InnerException.Message, Is.EqualTo(
				"Null value cannot be assigned to a value type 'System.Int32'. Cast expression '[a].FatherSerialNumber.Length' to 'System.Nullable`1[System.Int32]'."));

			var list2 = db.Animals.Select(
				a => new
				{
					ServerSide = a.Father.SerialNumber.Length.ToString(),
					ClientSide = a.FatherSerialNumber.Length.ToString()
				}).ToList();
			Assert.That(list2.Select(o => o.ClientSide), Is.EqualTo(list2.Select(o => o.ServerSide)));

			var list3 = db.Animals.Select(
				a => new
				{
					ServerSide = (int?) a.Father.SerialNumber.Substring(0, ((int?) a.Father.SerialNumber.Length - 1) ?? 0).Length,
					ClientSide = (int?) a.FatherSerialNumber.Substring(0, ((int?) a.FatherSerialNumber.Length - 1) ?? 0).Length
				}).ToList();
			Assert.That(list3.Select(o => o.ClientSide), Is.EqualTo(list3.Select(o => o.ServerSide)));

			var list4 = db.Animals.Select(a => new
			{
				ServerSide = a.Father.SerialNumber,
				ClientSide = a.FatherSerialNumber,
				Test = (object) null
			}).ToList();
			Assert.That(list4.Select(o => o.ClientSide), Is.EqualTo(list4.Select(o => o.ServerSide)));

			var list5 = db.Animals.Select(a => new
			{
				ServerSide = a.Father.SerialNumber == null,
				ClientSide = a.FatherSerialNumber == null
			}).ToList();
			Assert.That(list5.Select(o => o.ClientSide), Is.EqualTo(list5.Select(o => o.ServerSide)));

			var list6 = db.Animals
						  .Where(a => a.Father.SerialNumber != null)
						  .Select(
							  a => new
							  {
								  ServerSide = -a.Father.SerialNumber.Length,
								  ClientSide = -a.FatherSerialNumber.Length
							  }).ToList();
			Assert.That(list6.Select(o => o.ClientSide), Is.EqualTo(list6.Select(o => o.ServerSide)));

			var list7 = db.Animals
						  .Select(
							  a => new
							  {
								  ServerSide = a.Father != null ? a.Father.SerialNumber : null,
								  ClientSide = a.HasFather ? a.FatherSerialNumber : null
							  }).ToList();
			Assert.That(list7.Select(o => o.ClientSide), Is.EqualTo(list7.Select(o => o.ServerSide)));

			var list8 = db.Animals
			              .Where(a => a is Dog)
			              .Select(
				              a => new
				              {
					              ServerSide = (long?) (int?) ((Dog) a).Father.SerialNumber.Length,
					              ClientSide = (long?) (int?) ((Dog) a).FatherSerialNumber.Length
				              }).ToList();
			Assert.That(list8.Select(o => o.ClientSide), Is.EqualTo(list8.Select(o => o.ServerSide)));
		}

		public class Wrapper<T>
		{
			public T item;
			public string message;
		}

		private double GetTolerance()
		{
			return !Dialect.SupportsIEEE754FloatingPointNumbers || TestDialect.SendsParameterValuesAsStrings
				? 0.1d
				: 0d;
		}

		private static void AssertOneSelectColumn(IQueryable query)
		{
			using (var sqlLog = new SqlLogSpy())
			{
				// Execute query
				foreach (var item in query) { }
				Assert.That(FindAllOccurrences(sqlLog.GetWholeLog(), "as col"), Is.EqualTo(1));
			}
		}

		private static string GetSqlSelect(IQueryable query)
		{
			using (var sqlLog = new SqlLogSpy())
			{
				// Execute query
				foreach (var item in query) { }

				var sql = sqlLog.GetWholeLog();
				return sql.Substring(0, sql.IndexOf(" from"));
			}
		}

		private static int FindAllOccurrences(string source, string substring)
		{
			if (source == null)
			{
				return 0;
			}
			int n = 0, count = 0;
			while ((n = source.IndexOf(substring, n, StringComparison.InvariantCulture)) != -1)
			{
				n += substring.Length;
				++count;
			}
			return count;
		}
	}
}
