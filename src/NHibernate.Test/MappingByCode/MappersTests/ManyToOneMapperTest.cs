using System;
using System.Linq;
using NHibernate.Mapping.ByCode;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode.Impl;
using NUnit.Framework;

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
			Assert.That(hbm.cascade.Split(',').Select(w => w.Trim()), Contains.Item("persist").And.Contains("delete"));
		}

		[Test]
		public void CanSetAccessor()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Access(Accessor.ReadOnly);
			Assert.That(hbm.Access, Is.EqualTo("readonly"));
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);
			mapper.Column(cm => cm.Name("RelationId"));

			Assert.That(hbm.Columns.Count(), Is.EqualTo(1));
			Assert.That(hbm.Columns.Single().name, Is.EqualTo("RelationId"));
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Column(cm => cm.Name("Relation"));
			Assert.That(mapping.column, Is.Null);
			Assert.That(mapping.Columns, Is.Empty);
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
			Assert.That(mapping.Items, Is.Null);
			Assert.That(mapping.uniquekey, Is.EqualTo("theUnique"));
			Assert.That(mapping.notnull, Is.EqualTo(true));
			Assert.That(mapping.notnullSpecified, Is.EqualTo(true));
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
			Assert.That(mapping.Items, Is.Not.Null);
			Assert.That(mapping.Columns.Count(), Is.EqualTo(1));
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

			Assert.That(mapping.Items, Is.Null);
			Assert.That(mapping.uniquekey, Is.EqualTo("theUnique"));
			Assert.That(mapping.notnull, Is.EqualTo(true));
			Assert.That(mapping.notnullSpecified, Is.EqualTo(true));
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
			Assert.That(mapping.Columns.Count(), Is.EqualTo(2));
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAutoassignColumnNames()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			Assert.That(mapping.Columns.Count(), Is.EqualTo(2));
			Assert.That(mapping.Columns.All(cm => !string.IsNullOrEmpty(cm.name)), Is.True);
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			Assert.That(() => mapper.Column(cm => cm.Length(50)), Throws.TypeOf<MappingException>());
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

			Assert.That(mapping.Items, Is.Null);
			Assert.That(mapping.column, Is.EqualTo("pizza"));
			Assert.That(mapping.notnull, Is.EqualTo(true));
			Assert.That(mapping.unique, Is.EqualTo(true));
			Assert.That(mapping.uniquekey, Is.EqualTo("AA"));
			Assert.That(mapping.index, Is.EqualTo("II"));
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

			Assert.That(mapping.fetch, Is.EqualTo(HbmFetchMode.Join));
			Assert.That(mapping.fetchSpecified, Is.True);
		}

		[Test]
		public void WhenSetFetchModeToSelectThenResetFetch()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Fetch(FetchKind.Select);

			Assert.That(mapping.fetch, Is.EqualTo(HbmFetchMode.Select));
			Assert.That(mapping.fetchSpecified, Is.False);
		}

		[Test]
		public void CanForceClassRelation()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			mapper.Class(typeof(Relation));

			Assert.That(mapping.Class, Is.StringContaining("Relation").And.Not.Contains("IRelation"));
		}

		[Test]
		public void WhenForceClassRelationToIncompatibleTypeThenThrows()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);

			Assert.That(() => mapper.Class(typeof(Whatever)), Throws.TypeOf<ArgumentOutOfRangeException>());
		}

		[Test]
		public void CanSetLazyness()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("TheOtherRelation");
			var mapping = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, mapping, hbmMapping);
			mapper.Lazy(LazyRelation.NoProxy);
			Assert.That(mapping.Lazy, Is.Not.Null);
			Assert.That(mapping.Lazy, Is.EqualTo(HbmLaziness.NoProxy));
		}

		[Test]
		public void CanSetUpdate()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Update(false);
			Assert.That(hbm.update, Is.False);
		}

		[Test]
		public void CanSetInsert()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.Insert(false);
			Assert.That(hbm.insert, Is.False);
		}

		[Test]
		public void CanSetFk()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.ForeignKey("MyFkName");

			Assert.That(hbm.foreignkey, Is.EqualTo("MyFkName"));
		}

		[Test]
		public void CanSetPropertyRefName()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.PropertyRef("PropertyRefName");

			Assert.That(hbm.propertyref, Is.EqualTo("PropertyRefName"));
		}

		[Test]
		public void CanSetNotFoundWithExceptionMode()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.NotFound(NotFoundMode.Exception);

			Assert.That(hbm.notfound, Is.EqualTo(HbmNotFoundMode.Exception));
		}

		[Test]
		public void CanSetNotFoundWithIgnoreMode()
		{
			var hbmMapping = new HbmMapping();
			var member = typeof(MyClass).GetProperty("Relation");
			var hbm = new HbmManyToOne();
			var mapper = new ManyToOneMapper(member, hbm, hbmMapping);

			mapper.NotFound(NotFoundMode.Ignore);

			Assert.That(hbm.notfound, Is.EqualTo(HbmNotFoundMode.Ignore));
		}
	}
}