using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture(typeof(EnumType<TestEnum>))]
	[TestFixture(typeof(EnumStringType<TestEnum>))]
	[TestFixture(typeof(EnumAnsiStringType<TestEnum>))]
	public class EnumTests : TestCaseMappingByCode
	{
		private IType _enumType;


		public EnumTests(System.Type enumType)
		{
			_enumType = (IType) Activator.CreateInstance(enumType);
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EnumEntity>(
				rc =>
				{
					rc.Table("EnumEntity");
					rc.Id(x => x.Id, m => m.Generator(Generators.Identity));
					rc.Property(x => x.Name);
					rc.Property(x => x.Enum, m => m.Type(_enumType));
					rc.Property(x => x.NullableEnum, m => m.Type(_enumType));
					rc.ManyToOne(x => x.Other, m => m.Cascade(Mapping.ByCode.Cascade.All));
				});


			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				session.Save(new EnumEntity { Enum = TestEnum.Unspecified });
				session.Save(new EnumEntity { Enum = TestEnum.Small });
				session.Save(new EnumEntity { Enum = TestEnum.Small });
				session.Save(new EnumEntity { Enum = TestEnum.Medium });
				session.Save(new EnumEntity { Enum = TestEnum.Medium });
				session.Save(new EnumEntity { Enum = TestEnum.Medium });
				session.Save(new EnumEntity { Enum = TestEnum.Large });
				session.Save(new EnumEntity { Enum = TestEnum.Large });
				session.Save(new EnumEntity { Enum = TestEnum.Large });
				session.Save(new EnumEntity { Enum = TestEnum.Large });
				trans.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				session.Delete("from System.Object");

				session.Flush();
				transaction.Commit();
			}
		}

		[Test]
		public void CanQueryOnEnum_Large_4()
		{
			CanQueryOnEnum(TestEnum.Large, 4);
		}

		[Test]
		public void CanQueryOnEnum_Medium_3()
		{
			CanQueryOnEnum(TestEnum.Medium, 3);
		}

		[Test]
		public void CanQueryOnEnum_Small_2()
		{
			CanQueryOnEnum(TestEnum.Small, 2);
		}

		[Test]
		public void CanQueryOnEnum_Unspecified_1()
		{
			CanQueryOnEnum(TestEnum.Unspecified, 1);
		}

		private void CanQueryOnEnum(TestEnum type, int expectedCount)
		{
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var query = session.Query<EnumEntity>().Where(x => x.Enum == type).ToList();

				Assert.AreEqual(expectedCount, query.Count);
			}
		}

		[Test]
		public void CanQueryWithContainsOnTestEnum_Small_1()
		{
			var values = new[] { TestEnum.Small, TestEnum.Medium };
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var query = session.Query<EnumEntity>().Where(x => values.Contains(x.Enum)).ToList();

				Assert.AreEqual(5, query.Count);
			}
		}

		[Test]
		public void ConditionalNavigationProperty()
		{
			TestEnum? type = null;
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var entities = session.Query<EnumEntity>();
				entities.Where(o => o.Enum == TestEnum.Large).ToList();
				entities.Where(o => TestEnum.Large != o.Enum).ToList();
				entities.Where(o => (o.NullableEnum ?? TestEnum.Large) == TestEnum.Medium).ToList();
				entities.Where(o => ((o.NullableEnum ?? type) ?? o.Enum) == TestEnum.Medium).ToList();

				entities.Where(o => (o.NullableEnum.HasValue ? o.Enum : TestEnum.Unspecified) == TestEnum.Medium).ToList();
				entities.Where(o => (o.Enum != TestEnum.Large
										? (o.NullableEnum.HasValue ? o.Enum : TestEnum.Unspecified)
										: TestEnum.Small) == TestEnum.Medium).ToList();

				entities.Where(o => (o.Enum == TestEnum.Large ? o.Other : o.Other).Name == "test").ToList();
			}
		}

		[Test]
		public void CanQueryComplexExpressionOnTestEnum()
		{
			var type = TestEnum.Unspecified;
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var entities = session.Query<EnumEntity>();

				var query = (from user in entities
							 where (user.NullableEnum == TestEnum.Large
									   ? TestEnum.Medium
									   : user.NullableEnum ?? user.Enum
								   ) == type
							 select new
							 {
								 user,
								 simple = user.Enum,
								 condition = user.Enum == TestEnum.Large ? TestEnum.Medium : user.Enum,
								 coalesce = user.NullableEnum ?? TestEnum.Medium
							 }).ToList();

				Assert.That(query.Count, Is.EqualTo(1));
			}
		}

		public class EnumEntity
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }

			public virtual TestEnum Enum { get; set; }
			public virtual TestEnum? NullableEnum { get; set; }

			public virtual EnumEntity Other { get; set; }
		}

		public enum TestEnum
		{
			Unspecified,
			Small,
			Medium,
			Large
		}

		[Serializable]
		public class EnumAnsiStringType<T> : EnumStringType
		{
			private readonly string typeName;

			public EnumAnsiStringType()
				: base(typeof(T))
			{
				System.Type type = GetType();
				typeName = type.FullName + ", " + type.Assembly.GetName().Name;
			}

			public override string Name
			{
				get { return typeName; }
			}

			public override SqlType SqlType => SqlTypeFactory.GetAnsiString(255);
		}
	}
}
