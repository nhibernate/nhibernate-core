using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NHibernate.Mapping.ByCode;
using NHibernate.Type;
using NUnit.Framework;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Base class for fixtures testing individual types, created to avoid
	/// code duplication in derived classes.
	/// </summary>
	public abstract class GenericTypeFixtureBase<TProperty, TType> : TestCase where TType : IType
	{

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get
			{
				return new string[] { };
			}
		}

		/// <summary>
		/// Creates a set of test values of type <typeparamref name="TProperty"/>.
		/// </summary>
		/// <returns>The test values</returns>
		/// <remarks>If the type is IComparable, make sure that the first value
		/// doesn't become invalid when incremented by <see cref="IncrementValue(TProperty)"/></remarks>
		protected abstract IReadOnlyList<TProperty> TestValues { get; }

		/// <summary>
		/// Override in order to adjust the value of a property before comparisons.
		/// May be necessary when dealing with expected precision losses
		/// </summary>
		/// <param name="value">The value to adjust</param>
		/// <returns>The adjusted value</returns>
		protected virtual TProperty AdjustValue(TProperty value) => value;

		/// <summary>
		/// Sub properties of <see cref="TType"/> which should be queryable using LINQ
		/// </summary>
		protected virtual IEnumerable<Expression<Func<TProperty, object>>> PropertiesToTestWithLinq => Enumerable.Empty<Expression<Func<TProperty, object>>>();

		/// <summary>
		/// Methods of <see cref="TType"/> which should be queryable using LINQ
		/// </summary>
		protected virtual IEnumerable<Expression<Func<TProperty, object>>> MethodsToTestWithLinq => Enumerable.Empty<Expression<Func<TProperty, object>>>();

		[Test]
		public virtual void CanPersist()
		{
			var testValues = GetAllTestValues();

			Dictionary<Guid, TProperty> expectedValues = [];
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				foreach (var testValue in testValues)
				{
					var entity = new TestEntity { TestProperty = testValue };
					session.Save(entity);
					expectedValues[entity.Id] = testValue;
				}
				trans.Commit();
			}

			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				foreach (var expectedValue in expectedValues)
				{
					var entity = session.Get<TestEntity>(expectedValue.Key);
					Assert.That(entity, Is.Not.Null);
					Assert.That(AdjustValue(entity.TestProperty), Is.EqualTo(AdjustValue(expectedValue.Value)));
				}
			}
		}

		[Test]
		public void CanQuery()
		{
			var testValue = GetFirstTestValue();
			Guid id;
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var entity = new TestEntity { TestProperty = testValue };
				session.Save(entity);
				id = entity.Id;

				trans.Commit();
			}

			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var param = Expression.Parameter(typeof(TestEntity));
				var prop = Expression.Property(param, nameof(TestEntity.TestProperty));
				var value = Expression.Constant(testValue);
				var where = Expression.Lambda<Func<TestEntity, bool>>(Expression.Equal(prop, value), param);
				var entity = session.Query<TestEntity>().Single(where);
				Assert.That(entity, Is.Not.Null);
				Assert.That(entity.Id, Is.EqualTo(id));
			}
		}

		private TProperty GetFirstTestValue()
		{
			var testValues = TestValues;
			if (testValues.Count == 0)
			{
				Assert.Ignore("No test values provided");
			}
			return testValues[0];
		}

		private IReadOnlyList<TProperty> GetAllTestValues()
		{
			var testValues = TestValues;
			if (TestValues.Count == 0)
			{
				Assert.Ignore("No test values provided");
			}
			return testValues;
		}

		[Test]
		public virtual void CanCompare()
		{
			if (!typeof(IComparable).IsAssignableFrom(typeof(TProperty)))
			{
				Assert.Ignore("Not IComparable");
			}
			var testValues = GetAllTestValues();

			if (testValues.Count < 2)
			{
				Assert.Fail("At least 2 test values required to test comparison");
			}
			var testValue = testValues[0];
			var biggerTestValue = testValues[1];
			if (((IComparable) biggerTestValue).CompareTo(testValue) <= 0)
			{
				Assert.Fail("The second test value must be greater than the first");
				return;
			}
			Guid id;
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var entity = new TestEntity { TestProperty = testValue };
				session.Save(entity);
				id = entity.Id;
				entity = new TestEntity { TestProperty = biggerTestValue };
				session.Save(entity);
				trans.Commit();
			}

			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var param = Expression.Parameter(typeof(TestEntity));
				var prop = Expression.Property(param, nameof(TestEntity.TestProperty));
				var smallerValue = Expression.Constant(testValue, typeof(TProperty));
				var biggerValue = Expression.Constant(biggerTestValue, typeof(TProperty));
				var smallerWhere = Expression.Lambda<Func<TestEntity, bool>>(Expression.LessThan(prop, biggerValue), param);
				var biggerWhere = Expression.Lambda<Func<TestEntity, bool>>(Expression.GreaterThan(prop, smallerValue), param);
				var smaller = session.Query<TestEntity>().Single(smallerWhere);
				var bigger = session.Query<TestEntity>().Single(biggerWhere);
				Assert.That(smaller, Is.Not.Null);
				Assert.That(smaller.Id, Is.EqualTo(id));
				Assert.That(bigger, Is.Not.Null);
				Assert.That(bigger.Id, Is.Not.EqualTo(id));
			}
		}

		[Test]
		public virtual void CanQueryProperties()
		{
			if (PropertiesToTestWithLinq?.Any() != true)
			{
				Assert.Ignore();
			}

			var testValue = GetFirstTestValue();
			Guid id;
			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				var entity = new TestEntity { TestProperty = testValue };
				session.Save(entity);
				id = entity.Id;
				trans.Commit();
			}

			using (var session = OpenSession())
			using (var trans = session.BeginTransaction())
			{
				foreach (var property in PropertiesToTestWithLinq)
				{
					var param = Expression.Parameter(typeof(TestEntity));
					var body = property.Body;
					if (body is UnaryExpression unaryExpression)
					{
						body = unaryExpression.Operand;
					}
					var member = body as MemberExpression;
					if (member is null)
					{
						Assert.Fail(body + " did not expose a member");
					}
					var prop = Expression.Property(param, nameof(TestEntity.TestProperty));
					var value = property.Compile()(testValue);
					var where = Expression.Lambda<Func<TestEntity, bool>>(Expression.Equal(body.Replace(property.Parameters[0], prop), Expression.Constant(value)), param);
					TestEntity entity = null;
					Assert.DoesNotThrow(() => entity = session.Query<TestEntity>().FirstOrDefault(where), "Unable to query property " + member.Member.Name);
					Assert.That(entity, Is.Not.Null, "Unable to query property " + member.Member.Name);
				}
			}
		}

		protected override void AddMappings(Configuration configuration)
		{
			var mapper = new ModelMapper();

			mapper.Class<TestEntity>(m =>
			{
				m.Table("TestEntity");
				m.EntityName("TestEntity");
				m.Id(p => p.Id, p => p.Generator(Generators.Guid));
				m.Property(p => p.TestProperty,
					p =>
					{
						p.Type<TType>();
						ConfigurePropertyMapping<TType>(p);
					}
				);
			});

			var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();
			configuration.AddMapping(mapping);
		}

		protected virtual void ConfigurePropertyMapping<TPersistentType>(IPropertyMapper propertyMapper) { }

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using var s = OpenSession();
			using var t = s.BeginTransaction();
			s.Query<TestEntity>().Delete();
			t.Commit();
		}
		public class TestEntity
		{
			public virtual Guid Id { get; set; }
			public virtual TProperty TestProperty { get; set; }
		}

		public class TypeConfiguration
		{
			public object Parameters { get; set; }
			public short? Scale { get; set; }
			public short? Precision { get; set; }
		}
	}
}
