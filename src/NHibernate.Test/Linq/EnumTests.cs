using System;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture(typeof(EnumType<TestEnum>))]
	[TestFixture(typeof(EnumStringType<TestEnum>))]
	[TestFixture(typeof(EnumAnsiStringType<TestEnum>))]
	public class EnumTests : TestCaseMappingByCode
	{
		private IType _enumType;
		private ISession _session;

		private IQueryable<EnumEntity> TestEntities { get; set; }
		private IQueryable<EnumEntity> TestEntitiesInDb { get; set; }

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
					rc.Property(x => x.BatchId);
					rc.Property(x => x.Enum, m => m.Type(_enumType));
					rc.Property(x => x.NullableEnum, m => m.Type(_enumType));
					rc.ManyToOne(x => x.Other, m => m.Cascade(Mapping.ByCode.Cascade.All));
				});


			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void OnSetUp()
		{
			base.OnSetUp();
			_session = OpenSession();

			var entities = new[] {
				new EnumEntity { Enum = TestEnum.Unspecified },
				new EnumEntity { Enum = TestEnum.Small, NullableEnum = TestEnum.Large },
				new EnumEntity { Enum = TestEnum.Small, NullableEnum = TestEnum.Medium },
				new EnumEntity { Enum = TestEnum.Medium, NullableEnum = TestEnum.Medium },
				new EnumEntity { Enum = TestEnum.Medium, NullableEnum = TestEnum.Small },
				new EnumEntity { Enum = TestEnum.Medium },
				new EnumEntity { Enum = TestEnum.Large, NullableEnum = TestEnum.Medium },
				new EnumEntity { Enum = TestEnum.Large, NullableEnum = TestEnum.Unspecified },
				new EnumEntity { Enum = TestEnum.Large },
				new EnumEntity { Enum = TestEnum.Large }
			};

			var batchId = Guid.NewGuid();

			using (var trans = _session.BeginTransaction())
			{
				foreach (var item in entities)
				{
					item.BatchId = batchId;
					_session.Save(item);
				}
				trans.Commit();
			}

			TestEntitiesInDb = _session.Query<EnumEntity>().Where(x=>x.BatchId == batchId);
			TestEntities = entities.AsQueryable();
		}

		protected override void OnTearDown()
		{
			if (_session.IsOpen)
			{
				_session.Close();
			}
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
			var query = TestEntitiesInDb.Where(x => x.Enum == type).ToList();

			Assert.AreEqual(expectedCount, query.Count);
		}

		[Test]
		public void CanQueryWithContainsOnTestEnum_Small_1()
		{
			var values = new[] { TestEnum.Small, TestEnum.Medium };

			var query = TestEntitiesInDb.Where(x => values.Contains(x.Enum)).ToList();

			Assert.AreEqual(5, query.Count);
		}

		[Test]
		public void ConditionalNavigationProperty()
		{
			TestEnum? type = null;


			TestEntitiesInDb.Where(o => o.Enum == TestEnum.Large).ToList();
			TestEntitiesInDb.Where(o => TestEnum.Large != o.Enum).ToList();
			TestEntitiesInDb.Where(o => (o.NullableEnum ?? TestEnum.Large) == TestEnum.Medium).ToList();
			TestEntitiesInDb.Where(o => ((o.NullableEnum ?? type) ?? o.Enum) == TestEnum.Medium).ToList();

			TestEntitiesInDb.Where(o => (o.NullableEnum.HasValue ? o.Enum : TestEnum.Unspecified) == TestEnum.Medium).ToList();
			TestEntitiesInDb.Where(o => (o.Enum != TestEnum.Large
										? (o.NullableEnum.HasValue ? o.Enum : TestEnum.Unspecified)
										: TestEnum.Small) == TestEnum.Medium).ToList();

			TestEntitiesInDb.Where(o => (o.Enum == TestEnum.Large ? o.Other : o.Other).Name == "test").ToList();
		}

		[Test]
		public void CanQueryComplexExpressionOnTestEnum()
		{
			var type = TestEnum.Unspecified;

			Expression<Func<EnumEntity, bool>> predicate = user => (user.NullableEnum == TestEnum.Large
									? TestEnum.Medium
									: user.NullableEnum ?? user.Enum
								) == type;

			var query = TestEntitiesInDb.Where(predicate).Select(entity => new ProjectedEnum
			{
				Entity = entity,
				Simple = entity.Enum,
				Condition = entity.Enum == TestEnum.Large ? TestEnum.Medium : entity.Enum,
				Coalesce = entity.NullableEnum ?? TestEnum.Medium
			}).ToList();

			var targetCount = TestEntities.Count(predicate); //the truth
			Assert.That(targetCount, Is.GreaterThan(0)); //test sanity check
			Assert.That(query.Count, Is.EqualTo(targetCount));

			Assert.That(query, Is.All.Matches<ProjectedEnum>(x => x.Simple == x.Entity.Enum));
			Assert.That(query, Is.All.Matches<ProjectedEnum>(x => x.Condition == (x.Entity.Enum == TestEnum.Large ? TestEnum.Medium : x.Entity.Enum)));
			Assert.That(query, Is.All.Matches<ProjectedEnum>(x => x.Coalesce == (x.Entity.NullableEnum ?? TestEnum.Medium)));

		}

		public class EnumEntity
		{
			public virtual int Id { get; set; }
			public virtual string Name { get; set; }

			public virtual TestEnum Enum { get; set; }
			public virtual TestEnum? NullableEnum { get; set; }

			public virtual EnumEntity Other { get; set; }
			public virtual Guid BatchId { get; set; }
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

		private class ProjectedEnum
		{
			public TestEnum Simple { get; internal set; }
			public TestEnum Condition { get; internal set; }
			public TestEnum Coalesce { get; internal set; }
			public EnumEntity Entity { get; internal set; }
		}
	}
}
