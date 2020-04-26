using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class EnumTests : LinqTestCase
	{
		[Test]
		public void CanQueryOnEnumStoredAsInt32_High_1()
		{
			CanQueryOnEnumStoredAsInt32(EnumStoredAsInt32.High, 1);
		}

		[Test]
		public void CanQueryOnEnumStoredAsInt32_Unspecified_2()
		{
			CanQueryOnEnumStoredAsInt32(EnumStoredAsInt32.Unspecified, 2);
		}

		public void CanQueryOnEnumStoredAsInt32(EnumStoredAsInt32 type, int expectedCount)
		{
			var query = (from user in db.Users
						 where user.Enum2 == type
						 select user).ToList();

			Assert.AreEqual(expectedCount, query.Count);
		}

		[Test]
		public void CanQueryOnEnumStoredAsString_Meduim_2()
		{
			CanQueryOnEnumStoredAsString(EnumStoredAsString.Medium, 2);
		}

		[Test]
		public void CanQueryOnEnumStoredAsString_Small_1()
		{
			CanQueryOnEnumStoredAsString(EnumStoredAsString.Small, 1);
		}

		public void CanQueryOnEnumStoredAsString(EnumStoredAsString type, int expectedCount)
		{
			var query = (from user in db.Users
						 where user.Enum1 == type
						 select user).ToList();

			Assert.AreEqual(expectedCount, query.Count);
		}

		[Test]
		public void ConditionalNavigationProperty()
		{
			EnumStoredAsString? type = null;
			db.Users.Where(o => o.Enum1 == EnumStoredAsString.Large).ToList();
			db.Users.Where(o => EnumStoredAsString.Large != o.Enum1).ToList();
			db.Users.Where(o => (o.NullableEnum1 ?? EnumStoredAsString.Large) == EnumStoredAsString.Medium).ToList();
			db.Users.Where(o => ((o.NullableEnum1 ?? type) ?? o.Enum1) == EnumStoredAsString.Medium).ToList();

			db.Users.Where(o => (o.NullableEnum1.HasValue ? o.Enum1 : EnumStoredAsString.Unspecified) == EnumStoredAsString.Medium).ToList();
			db.Users.Where(o => (o.Enum1 != EnumStoredAsString.Large
				                    ? (o.NullableEnum1.HasValue ? o.Enum1 : EnumStoredAsString.Unspecified)
				                    : EnumStoredAsString.Small) == EnumStoredAsString.Medium).ToList();

			db.Users.Where(o => (o.Enum1 == EnumStoredAsString.Large ? o.Role : o.Role).Name == "test").ToList();
		}

		[Test]
		public void CanQueryComplexExpressionOnEnumStoredAsString()
		{
			var type = EnumStoredAsString.Unspecified;
			var query = (from user in db.Users
			             where (user.NullableEnum1 == EnumStoredAsString.Large
				                   ? EnumStoredAsString.Medium
				                   : user.NullableEnum1 ?? user.Enum1
			                   ) == type
			             select new
			             {
				             user,
				             simple = user.Enum1,
				             condition = user.Enum1 == EnumStoredAsString.Large ? EnumStoredAsString.Medium : user.Enum1,
				             coalesce = user.NullableEnum1 ?? EnumStoredAsString.Medium
			             }).ToList();

			Assert.That(query.Count, Is.EqualTo(0));
		}
	}
}
