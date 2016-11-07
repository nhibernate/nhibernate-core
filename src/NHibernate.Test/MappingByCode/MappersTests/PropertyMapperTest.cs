using System;
using System.Linq;
using System.Data;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Impl;
using NHibernate.Properties;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.MappersTests
{
	public class PropertyMapperTest
	{
		private enum MyEnum
		{
			One
		}
		private class MyClass
		{
			public string Autoproperty { get; set; }
			public string ReadOnly { get { return ""; } }
			public MyEnum EnumProp { get; set; }
		}

		private class MyAccessorMapper : IAccessorPropertyMapper
		{
			public bool AccessorCalled { get; set; }
			public void Access(Accessor accessor)
			{
				AccessorCalled = true;
			}

			public void Access(System.Type accessorType)
			{
				
			}
		}
		[Test]
		public void WhenCreateWithGivenAccessorMapperThenUseTheGivenAccessoMapper()
		{
			var member = typeof (MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var myAccessorMapper = new MyAccessorMapper();
			var mapper = new PropertyMapper(member, mapping, myAccessorMapper);
			mapper.Access(Accessor.Field);
			Assert.That(myAccessorMapper.AccessorCalled, Is.True);
		}

		[Test]
		public void WhenSettingByTypeThenCheckCompatibility()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			Assert.That(() => mapper.Access(typeof(object)), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(() => mapper.Access(typeof(FieldAccessor)), Throws.Nothing);
			Assert.That(mapping.Access, Is.EqualTo(typeof(FieldAccessor).AssemblyQualifiedName));
		}

		[Test]
		public void WhenSetTypeByITypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type(NHibernateUtil.String);

			Assert.That(mapping.Type.name, Is.EqualTo("String"));
		}

		[Test]
		public void WhenSetTypeByIUserTypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<MyType>();

			Assert.That(mapping.Type.name, Is.StringContaining("MyType"));
			Assert.That(mapping.type, Is.Null);
		}

		[Test]
		public void WhenSetTypeByICompositeUserTypeThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<MyCompoType>();

			Assert.That(mapping.Type.name, Is.StringContaining("MyCompoType"));
			Assert.That(mapping.type, Is.Null);
		}

		[Test]
		public void WhenSetTypeByIUserTypeWithParamsThenSetType()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<MyType>(new { Param1 = "a", Param2 = 12 });

			Assert.That(mapping.type1, Is.Null);
			Assert.That(mapping.Type.name, Is.StringContaining("MyType"));
			Assert.That(mapping.Type.param, Has.Length.EqualTo(2));
			Assert.That(mapping.Type.param.Select(p => p.name), Is.EquivalentTo(new [] {"Param1", "Param2"}));
			Assert.That(mapping.Type.param.Select(p => p.GetText()), Is.EquivalentTo(new [] {"a", "12"}));
		}

		[Test]
		public void WhenSetTypeByIUserTypeWithNullParamsThenSetTypeName()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<MyType>(null);

			Assert.That(mapping.Type.name, Is.StringContaining("MyType"));
			Assert.That(mapping.type, Is.Null);
		}

		[Test]
		public void WhenSetTypeByITypeTypeThenSetType()
		{
			var member = For<MyClass>.Property(c => c.EnumProp);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<EnumStringType<MyEnum>>();

			Assert.That(mapping.Type.name, Is.StringContaining(typeof(EnumStringType<MyEnum>).FullName));
			Assert.That(mapping.type, Is.Null);
		}

		[Test]
		public void WhenSetInvalidTypeThenThrow()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			Assert.That(() => mapper.Type(typeof(object), null), Throws.TypeOf<ArgumentOutOfRangeException>());
			Assert.That(() => mapper.Type(null, null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public void WhenSetDifferentColumnNameThenSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => cm.Name("pepe"));

			Assert.That(mapping.Columns.Count(), Is.EqualTo(1));
			Assert.That(mapping.Columns.Single().name, Is.EqualTo("pepe"));
		}

		[Test]
		public void WhenSetDefaultColumnNameThenDoesNotSetTheName()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => { cm.Name("Autoproperty"); cm.Length(50); });
			Assert.That(mapping.column, Is.Null);
			Assert.That(mapping.length, Is.EqualTo("50"));
			Assert.That(mapping.Columns, Is.Empty);
		}

		[Test]
		public void WhenSetBasicColumnValuesThenSetPlainValues()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm =>
			{
				cm.Length(50);
				cm.NotNullable(true);
			});
			Assert.That(mapping.Items, Is.Null);
			Assert.That(mapping.length, Is.EqualTo("50"));
			Assert.That(mapping.notnull, Is.EqualTo(true));
			Assert.That(mapping.notnullSpecified, Is.EqualTo(true));
		}

		[Test]
		public void WhenSetColumnValuesThenAddColumnTag()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm =>
			{
				cm.SqlType("VARCHAR(50)");
				cm.NotNullable(true);
			});
			Assert.That(mapping.Items, Is.Not.Null);
			Assert.That(mapping.Columns.Count(), Is.EqualTo(1));
		}

		[Test]
		public void WhenSetBasicColumnValuesMoreThanOnesThenMergeColumn()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column(cm => cm.Length(50));
			mapper.Column(cm => cm.NotNullable(true));

			Assert.That(mapping.Items, Is.Null);
			Assert.That(mapping.length, Is.EqualTo("50"));
			Assert.That(mapping.notnull, Is.EqualTo(true));
			Assert.That(mapping.notnullSpecified, Is.EqualTo(true));
		}

		[Test]
		public void WhenSetMultiColumnsValuesThenAddColumns()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Type<MyType>();
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
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			Assert.That(mapping.Columns.Count(), Is.EqualTo(2));
			Assert.True(mapping.Columns.All(cm => !string.IsNullOrEmpty(cm.name)));
		}

		[Test]
		public void AfterSetMultiColumnsCantSetSimpleColumn()
		{
			var member = typeof(MyClass).GetProperty("ReadOnly");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Columns(cm => cm.Length(50), cm => cm.SqlType("VARCHAR(10)"));
			Assert.That(() => mapper.Column(cm => cm.Length(50)), Throws.TypeOf<MappingException>());
		}

		[Test]
		public void WhenSetBasicColumnValuesThroughShortCutThenMergeColumn()
		{
			var member = typeof(MyClass).GetProperty("Autoproperty");
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);
			mapper.Column("pizza");
			mapper.Length(50);
			mapper.Precision(10);
			mapper.Scale(2);
			mapper.NotNullable(true);
			mapper.Unique(true);
			mapper.UniqueKey("AA");
			mapper.Index("II");

			Assert.That(mapping.Items, Is.Null);
			Assert.That(mapping.column, Is.EqualTo("pizza"));
			Assert.That(mapping.length, Is.EqualTo("50"));
			Assert.That(mapping.precision, Is.EqualTo("10"));
			Assert.That(mapping.scale, Is.EqualTo("2"));
			Assert.That(mapping.notnull, Is.EqualTo(true));
			Assert.That(mapping.unique, Is.EqualTo(true));
			Assert.That(mapping.uniquekey, Is.EqualTo("AA"));
			Assert.That(mapping.index, Is.EqualTo("II"));
		}

		[Test]
		public void WhenSetUpdateThenSetAttributes()
		{
			var member = For<MyClass>.Property(x => x.ReadOnly);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Update(false);
			Assert.That(mapping.update, Is.False);
			Assert.That(mapping.updateSpecified, Is.True);
		}

		[Test]
		public void WhenSetInsertThenSetAttributes()
		{
			var member = For<MyClass>.Property(x => x.ReadOnly);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Insert(false);
			Assert.That(mapping.insert, Is.False);
			Assert.That(mapping.insertSpecified, Is.True);
		}

		[Test]
		public void WhenSetLazyThenSetAttributes()
		{
			var member = For<MyClass>.Property(x => x.ReadOnly);
			var mapping = new HbmProperty();
			var mapper = new PropertyMapper(member, mapping);

			mapper.Lazy(true);
			Assert.That(mapping.lazy, Is.True);
			Assert.That(mapping.IsLazyProperty, Is.True);
		}
	}

	public class MyType : IUserType
	{
		#region Implementation of IUserType

		public bool Equals(object x, object y)
		{
			throw new NotImplementedException();
		}

		public int GetHashCode(object x)
		{
			throw new NotImplementedException();
		}

		public object NullSafeGet(IDataReader rs, string[] names, object owner)
		{
			throw new NotImplementedException();
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index)
		{
			throw new NotImplementedException();
		}

		public object DeepCopy(object value)
		{
			throw new NotImplementedException();
		}

		public object Replace(object original, object target, object owner)
		{
			throw new NotImplementedException();
		}

		public object Assemble(object cached, object owner)
		{
			throw new NotImplementedException();
		}

		public object Disassemble(object value)
		{
			throw new NotImplementedException();
		}

		public SqlType[] SqlTypes
		{
			get { throw new NotImplementedException(); }
		}

		public System.Type ReturnedType
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsMutable
		{
			get { throw new NotImplementedException(); }
		}

		#endregion
	}

	public class MyCompoType : ICompositeUserType
	{
		public object GetPropertyValue(object component, int property)
		{
			throw new NotImplementedException();
		}

		public void SetPropertyValue(object component, int property, object value)
		{
			throw new NotImplementedException();
		}

		public bool Equals(object x, object y)
		{
			throw new NotImplementedException();
		}

		public int GetHashCode(object x)
		{
			throw new NotImplementedException();
		}

		public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object DeepCopy(object value)
		{
			throw new NotImplementedException();
		}

		public object Disassemble(object value, ISessionImplementor session)
		{
			throw new NotImplementedException();
		}

		public object Assemble(object cached, ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public object Replace(object original, object target, ISessionImplementor session, object owner)
		{
			throw new NotImplementedException();
		}

		public string[] PropertyNames
		{
			get { throw new NotImplementedException(); }
		}

		public IType[] PropertyTypes
		{
			get { throw new NotImplementedException(); }
		}

		public System.Type ReturnedClass
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsMutable
		{
			get { throw new NotImplementedException(); }
		}
	}
}