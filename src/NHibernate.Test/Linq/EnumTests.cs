using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture(typeof(EnumType<TestEnum>), "0")]
	[TestFixture(typeof(EnumStringType<TestEnum>), "'Unspecified'")]
	[TestFixture(typeof(EnumAnsiStringType<TestEnum>), "'Unspecified'")]
	public class EnumTests : TestCaseMappingByCode
	{
		private IType _enumType;
		private string _unspecifiedValue;

		public EnumTests(System.Type enumType, string unspecifiedValue)
		{
			_enumType = (IType) Activator.CreateInstance(enumType);
			_unspecifiedValue = unspecifiedValue;
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<EnumEntity>(
				rc =>
				{
					rc.Table("EnumEntity");
					rc.Id(x => x.Id, m => m.Generator(Generators.Guid));
					rc.Property(x => x.Name);
					rc.Property(x => x.Enum1, m => m.Type(_enumType));
					rc.Property(x => x.NullableEnum1, m =>
					{
						m.Type(_enumType);
						m.Formula($"(case when Enum1 = {_unspecifiedValue} then null else Enum1 end)");
					});
					rc.Bag(x => x.Children, m => 
						{
							m.Cascade(Mapping.ByCode.Cascade.All);
							m.Inverse(true);
						},
						a => a.OneToMany()
					);
					rc.ManyToOne(x => x.Other, m => m.Cascade(Mapping.ByCode.Cascade.All));
				});

			mapper.Class<EnumEntityChild>(
				rc =>
				{
					rc.Table("EnumEntityChild");
					rc.Id(x => x.Id, m => m.Generator(Generators.Guid));
					rc.Property(x => x.Name);
				});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				session.Save(new EnumEntity { Enum1 = TestEnum.Unspecified });
				session.Save(new EnumEntity { Enum1 = TestEnum.Small });
				session.Save(new EnumEntity { Enum1 = TestEnum.Small });
				session.Save(new EnumEntity { Enum1 = TestEnum.Medium });
				session.Save(new EnumEntity { Enum1 = TestEnum.Medium });
				session.Save(new EnumEntity { Enum1 = TestEnum.Medium });
				session.Save(new EnumEntity { Enum1 = TestEnum.Large });
				session.Save(new EnumEntity { Enum1 = TestEnum.Large });
				session.Save(new EnumEntity { Enum1 = TestEnum.Large });
				session.Save(new EnumEntity { Enum1 = TestEnum.Large });
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
				var query = session.Query<EnumEntity>().Where(x => x.Enum1 == type).ToList();

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
				var query = session.Query<EnumEntity>().Where(x => values.Contains(x.Enum1)).ToList();

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
				entities.Where(o => o.Enum1 == TestEnum.Large).ToList();
				entities.Where(o => TestEnum.Large != o.Enum1).ToList();
				entities.Where(o => (o.NullableEnum1 ?? TestEnum.Large) == TestEnum.Medium).ToList();
				entities.Where(o => ((o.NullableEnum1 ?? type) ?? o.Enum1) == TestEnum.Medium).ToList();

				entities.Where(o => (o.NullableEnum1.HasValue ? o.Enum1 : TestEnum.Unspecified) == TestEnum.Medium).ToList();
				entities.Where(o => (o.Enum1 != TestEnum.Large
										? (o.NullableEnum1.HasValue ? o.Enum1 : TestEnum.Unspecified)
										: TestEnum.Small) == TestEnum.Medium).ToList();

				entities.Where(o => (o.Enum1 == TestEnum.Large ? o.Other : o.Other).Name == "test").ToList();
			}
		}

		[TestCase(null)]
		[TestCase(TestEnum.Unspecified)]
		public void CanQueryComplexExpressionOnTestEnum(TestEnum? type)
		{
			using (var session = OpenSession())
			{
				var entities = session.Query<EnumEntity>();

				var query = (from user in entities
							 where (user.NullableEnum1 == TestEnum.Large
									   ? TestEnum.Medium
									   : user.NullableEnum1 ?? user.Enum1
								   ) == type
							 select new
							 {
								 user,
								 simple = user.Enum1,
								 condition = user.Enum1 == TestEnum.Large ? TestEnum.Medium : user.Enum1,
								 coalesce = user.NullableEnum1 ?? TestEnum.Medium
							 }).ToList();

				Assert.That(query.Count, Is.EqualTo(type == TestEnum.Unspecified ? 1 : 0));
			}
		}

		[Test]
		public void CanProjectWithListTransformation()
		{
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var entities = session.Query<EnumEntity>();

				var query = entities.Select(user => new
				{
					user.Name,
					simple = user.Enum1,
					children = user.Children,
					nullableEnum1IsLarge = user.NullableEnum1 == TestEnum.Large
				}).ToList();

				Assert.That(query.Count, Is.EqualTo(10));
			}
		}
	}

	public class EnumEntity
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }

		public virtual TestEnum Enum1 { get; set; }
		public virtual TestEnum? NullableEnum1 { get; set; }

		public virtual int? NullableInt { get; set; }

		public virtual EnumEntity Other { get; set; }

		public virtual IList<EnumEntityChild> Children { get; set; } = new List<EnumEntityChild>();
	}

	public class EnumEntityChild
	{
		public virtual Guid Id { get; set; }
		public virtual string Name { get; set; }
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
