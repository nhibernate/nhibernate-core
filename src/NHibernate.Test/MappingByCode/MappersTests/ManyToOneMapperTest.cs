using System;
using System.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class ManyToOneMapperTest
	{
		private class MyClass
		{
			public Relation Relation { get; set; }
			public IRelation TheOtherRelation { get; set; }
		}

		private interface IRelation
		{
		}

		private class Relation : IRelation
		{
		}

		private class Whatever
		{
		}

		[Test]
		public void AssignCascadeStyle()
		{
			var hbmMapping = new HbmMapping();
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(null, hbm, hbmMapping);
			mapper.Cascade(Mapping.ByCode.Cascade.Persist | Mapping.ByCode.Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).Should().Contain("persist").And.Contain("delete");
		}

		[Test]
		public void AutoCleanUnsupportedCascadeStyle()
		{
			var hbmMapping = new HbmMapping();
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(null, hbm, hbmMapping);
			mapper.Cascade(Mapping.ByCode.Cascade.Persist | Mapping.ByCode.Cascade.DeleteOrphans | Mapping.ByCode.Cascade.Remove);
			hbm.cascade.Split(',').Select(w => w.Trim()).All(w => w.Satisfy(cascade => !cascade.Contains("orphan")));
		}

		[Test]
		public void CanSetAccessor()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Access(Accessor.ReadOnly);
			hbm.Access.Should().Be("readonly");
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);
			mapper.Column(cm => cm.Name("RelationId"));

			hbm.Columns.Should().Have.Count.EqualTo(1);
			hbm.Columns.Single().name.Should().Be("RelationId");
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.Name("Relation"));
			mapping.column.Should().Be.Null();
			mapping.Columns.Should().Be.Empty();
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm =>
			{
				cm.UniqueKey("theUnique");
				cm.NotNullable(true);
			});
			mapping.Items.Should().Be.Null();
			mapping.uniquekey.Should().Be("theUnique");
			mapping.notnull.Should().Be(true);
			mapping.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm =>
			{
				cm.SqlType("BIGINT");
				cm.NotNullable(true);
			});
			mapping.Items.Should().Not.Be.Null();
			mapping.Columns.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenSetBasicColumnValuesMoreThanOnesThenMergeColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.UniqueKey("theUnique"));
			mapper.Column(cm => cm.NotNullable(true));

			mapping.Items.Should().Be.Null();
			mapping.uniquekey.Should().Be("theUnique");
			mapping.notnull.Should().Be(true);
			mapping.notnullSpecified.Should().Be(true);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm =>
			{
				cm.Name("column1");
				cm.Length(50);
			}, cm =>
			{
				cm.Name("column2");
				cm.SqlType("VARCHAR(10)");
			});
			mapping.Columns.Should().Have.Count.EqualTo(2);
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAutoassignColumnNames()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			mapping.Columns.Should().Have.Count.EqualTo(2);
			mapping.Columns.All(cm => cm.name.Satisfy(n => !string.IsNullOrEmpty(n)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			Executing.This(() => mapper.Column(cm => cm.Length(50))).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenSetBasicColumnValuesThroughShortCutThenMergeColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column("pizza");
			mapper.NotNullable(true);
			mapper.Unique(true);
			mapper.UniqueKey("AA");
			mapper.Index("II");

			mapping.Items.Should().Be.Null();
			mapping.column.Should().Be("pizza");
			mapping.notnull.Should().Be(true);
			mapping.unique.Should().Be(true);
			mapping.uniquekey.Should().Be("AA");
			mapping.index.Should().Be("II");
		}

		[Test(Description = "NH-3618")]
		public void SetUniqueToMultiColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(x => x.Name("pizza"), x => x.Name("pasta"));
			mapper.Unique(true);
			mapper.UniqueKey("AA");
			mapper.Index("II");

			Assert.That(mapping.Items, Is.Not.Null.And.Not.Empty);
			Assert.IsNull(mapping.column);
			Assert.IsTrue(mapping.unique);
			Assert.That(mapping.uniquekey, Is.EqualTo("AA"));
			Assert.That(mapping.index, Is.EqualTo("II"));
		}

		[Test]
		public void WhenSetFetchModeToJoinThenSetFetch()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Fetch(FetchKind.Join);

			mapping.fetch.Should().Be(HbmFetchMode.Join);
			mapping.fetchSpecified.Should().Be.True();
		}

		[Test]
		public void WhenSetFetchModeToSelectThenResetFetch()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Fetch(FetchKind.Select);

			mapping.fetch.Should().Be(HbmFetchMode.Select);
			mapping.fetchSpecified.Should().Be.False();
		}

		[Test]
		public void CanForceClassRelation()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Class(typeof(Relation));

			mapping.Class.Should().Contain("Relation").And.Not.Contain("IRelation");
		}

		[Test]
		public void WhenForceClassRelationToIncompatibleTypeThenThrows()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			Executing.This(() => mapper.Class(typeof(Whatever))).Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void CanSetLazyness()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Lazy(LazyRelation.NoProxy);
			mapping.Lazy.Should().Have.Value();
			mapping.Lazy.Should().Be(HbmLaziness.NoProxy);
		}

		[Test]
		public void CanSetUpdate()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Update(false);
			hbm.update.Should().Be.False();
		}

		[Test]
		public void CanSetInsert()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Insert(false);
			hbm.insert.Should().Be.False();
		}

		[Test]
		public void CanSetFk()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.ForeignKey("MyFkName");

			hbm.foreignkey.Should().Be("MyFkName");
		}

		[Test]
		public void CanSetPropertyRefName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.PropertyRef("PropertyRefName");

			hbm.propertyref.Should().Be("PropertyRefName");
		}

		[Test]
		public void CanSetNotFoundWithExceptionMode()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.NotFound(NotFoundMode.Exception);

			hbm.notfound.Should().Be(HbmNotFoundMode.Exception);
		}

		[Test]
		public void CanSetNotFoundWithIgnoreMode()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.NotFound(NotFoundMode.Ignore);

			hbm.notfound.Should().Be(HbmNotFoundMode.Ignore);
		}
	}
}