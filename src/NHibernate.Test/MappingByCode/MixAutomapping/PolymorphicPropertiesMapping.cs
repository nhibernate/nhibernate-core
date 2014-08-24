using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.MixAutomapping
{
	public class PolymorphicPropertiesMapping
	{
		const BindingFlags RootClassPropertiesBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy;
	
		private interface IBaseEntity
		{
			int Id { get; }
		}

		private class BaseEntity : IBaseEntity
		{
			public int Id { get; protected set; }
			public DateTime LastChange { get { return DateTime.Now; } }
		}

		private interface IProduct : IBaseEntity
		{
			string Description { get;}
		}

		private abstract class BaseProduct : BaseEntity, IProduct
		{
			public abstract string Description { get;  }
		}

		private class Product : BaseProduct
		{
			public override string Description
			{
				get { return "blah"; }
			}
		}

		[Test]
		public void WhenMapIdThroughBaseInterfaceThenFindIt()
		{
			var inspector = (IModelInspector)new SimpleModelInspector();
			var mapper = new ModelMapper(inspector);
			mapper.Class<IBaseEntity>(
				map =>
				{
					map.Id(x => x.Id, idmap => { });
				});
			inspector.IsPersistentId(typeof(Product).GetProperty("Id", RootClassPropertiesBindingFlags)).Should().Be.True();
		}

		[Test]
		public void WhenMapPropertyThroughBaseConcreteClassThenFindIt()
		{
			var inspector = (IModelInspector)new SimpleModelInspector();
			var mapper = new ModelMapper(inspector);

			mapper.Class<BaseEntity>(map => map.Property(x => x.LastChange));

			inspector.IsPersistentProperty(typeof(Product).GetProperty("LastChange", RootClassPropertiesBindingFlags)).Should().Be.True();
		}

		[Test]
		public void WhenMapPropertyThroughBaseInterfaceThenFindIt()
		{
			var inspector = (IModelInspector)new SimpleModelInspector();
			var mapper = new ModelMapper(inspector);

			mapper.Class<IProduct>(map => map.Property(x => x.Description));

			inspector.IsPersistentProperty(typeof(Product).GetProperty("Description", RootClassPropertiesBindingFlags)).Should().Be.True();
		}

		[Test]
		public void WhenMapPropertyThroughBaseAbstractClassThenFindIt()
		{
			var inspector = (IModelInspector)new SimpleModelInspector();
			var mapper = new ModelMapper(inspector);

			mapper.Class<BaseProduct>(map => map.Property(x => x.Description));

			inspector.IsPersistentProperty(typeof(Product).GetProperty("Description", RootClassPropertiesBindingFlags)).Should().Be.True();
		}

		[Test]
		public void WhenMapPropertyThroughClassThenFindIt()
		{
			var inspector = (IModelInspector)new SimpleModelInspector();
			var mapper = new ModelMapper(inspector);

			mapper.Class<Product>(map => map.Property(x => x.Description));

			inspector.IsPersistentProperty(typeof(Product).GetProperty("Description", RootClassPropertiesBindingFlags)).Should().Be.True();
		}
	}
}