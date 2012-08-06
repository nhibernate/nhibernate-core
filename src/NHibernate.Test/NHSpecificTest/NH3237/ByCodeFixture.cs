using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Linq;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using System;

namespace NHibernate.Test.NHSpecificTest.NH3237
{
	[TestFixture]
	public class ByCodeFixture : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<Entity>(rc =>
			{
				rc.Id(x => x.Id, m => m.Generator(Generators.GuidComb));
				rc.Property(x => x.DateTimeOffset, m => m.Type(typeof(DateTimeOffsetUserType), new DateTimeOffsetUserType(TimeSpan.FromHours(10))));
				rc.Property(x => x.TestEnum, m => m.Type(typeof(TestEnumUserType), null));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				var e1 = new Entity
				{
					DateTimeOffset = DateTimeOffset.Parse("2012-08-06 11:00 +10:00"),
					TestEnum = TestEnum.Zero
				};
				session.Save(e1);

				var e2 = new Entity
				{
					DateTimeOffset = DateTimeOffset.Parse("2012-08-06 12:00 +10:00"),
					TestEnum = TestEnum.One
				};
				session.Save(e2);

				var e3 = new Entity
				{
					DateTimeOffset = DateTimeOffset.Parse("2012-08-06 13:00 +10:00"),
					TestEnum = TestEnum.Two
				};
				session.Save(e3);

				session.Flush();
				transaction.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (ISession session = OpenSession())
			using (ITransaction transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void Test_That_DateTimeOffset_UserType_Can_Be_Used_For_Max_And_Min_Aggregates()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var minDateTimeOffset = session.Query<Entity>().Min(e => e.DateTimeOffset);
				Assert.AreEqual(DateTimeOffset.Parse("2012-08-06 11:00 +10:00"), minDateTimeOffset);

				var maxDateTimeOffset = session.Query<Entity>().Max(e => e.DateTimeOffset);
				Assert.AreEqual(DateTimeOffset.Parse("2012-08-06 13:00 +10:00"), maxDateTimeOffset);
			}
		}

		[Test]
		public void Test_That_Enum_Type_Can_Be_Used_For_Max_And_Min_Aggregates()
		{
			using (ISession session = OpenSession())
			using (session.BeginTransaction())
			{
				var minTestEnum = session.Query<Entity>().Min(e => e.TestEnum);
				Assert.AreEqual(TestEnum.Zero, minTestEnum);

				var maxTestEnum = session.Query<Entity>().Max(e => e.TestEnum);
				Assert.AreEqual(TestEnum.Two, maxTestEnum);
			}
		}
	}
}