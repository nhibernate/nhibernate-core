using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.MappingByCode.ExpliticMappingTests
{
	public class AllPropertiesRegistrationTests
	{
		private class MyClass
		{
			public int Id { get; set; }
			private int simple;
			public int Simple
			{
				get { return simple; }
				set { simple = value; }
			}

			private IList<string> complexType;
			public IList<string> ComplexType
			{
				get { return complexType; }
				set { complexType = value; }
			}

			private IEnumerable<string> bag;
			public IEnumerable<string> Bag
			{
				get { return bag; }
				set { bag = value; }
			}

			private IEnumerable<MyCompo> idBag;
			public IEnumerable<MyCompo> IdBag
			{
				get { return idBag; }
				set { idBag = value; }
			}

			private IEnumerable<string> set;
			public IEnumerable<string> Set
			{
				get { return set; }
				set { set = value; }
			}

			private IEnumerable<string> list;
			public IEnumerable<string> List
			{
				get { return list; }
				set { list = value; }
			}

			private IDictionary<int, string> map;
			public IDictionary<int, string> Map
			{
				get { return map; }
				set { map = value; }
			}

			private MyCompo compo;
			public MyCompo Compo
			{
				get { return compo; }
				set { compo = value; }
			}

			private Related oneToOne;
			public Related OneToOne
			{
				get { return oneToOne; }
				set { oneToOne = value; }
			}

			private Related manyToOne;
			public Related ManyToOne
			{
				get { return manyToOne; }
				set { manyToOne = value; }
			}

			private object any;
			public object Any
			{
				get { return any; }
				set { any = value; }
			}

			private IDictionary dynamicCompo;
			public IDictionary DynamicCompo
			{
				get { return dynamicCompo; }
				set { dynamicCompo = value; }
			}
		}

		private class MyCompo
		{
			public int Something { get; set; }
		}
		private class Related
		{
			public int  Id { get; set; }
		}

		private class Inherited:MyClass
		{
			
		}

		[Test]
		public void WhenMapPropertiesInTheBaseJumpedClassThenMapInInherited()
		{
			// ignoring MyClass and using Inherited, as root-class, I will try to map all properties using the base class.
			// NH have to recognize the case and map those properties in the inherited.
			var inspector = new SimpleModelInspector();
			inspector.IsEntity((type, declared) => type == typeof(Inherited));
			inspector.IsRootEntity((type, declared) => type == typeof(Inherited));
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(mc =>
			                      {
			                      	mc.Id(x => x.Id);
			                      	mc.Property(x => x.Simple, map => map.Access(Accessor.Field));
			                      	mc.Property(x => x.ComplexType, map => map.Access(Accessor.Field));
			                      	mc.Bag(x => x.Bag, y => y.Access(Accessor.Field));
			                      	mc.IdBag(x => x.IdBag, y => y.Access(Accessor.Field));
			                      	mc.List(x => x.List, y => y.Access(Accessor.Field));
			                      	mc.Set(x => x.Set, y => y.Access(Accessor.Field));
			                      	mc.Map(x => x.Map, y => y.Access(Accessor.Field));
															mc.OneToOne(x => x.OneToOne, y => y.Access(Accessor.Field));
															mc.ManyToOne(x => x.ManyToOne, y => y.Access(Accessor.Field));
															mc.Any(x => x.Any, typeof(int), y => y.Access(Accessor.Field));
															mc.Component(x => x.DynamicCompo, new { A = 2 }, y => y.Access(Accessor.Field));
															mc.Component(x => x.Compo, y =>
			                      	                           {
			                      	                           	y.Access(Accessor.Field);
			                      	                           	y.Property(c => c.Something);
			                      	                           });
			                      });
			mapper.Class<Inherited>(mc =>{});

			var mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			var hbmClass = mappings.RootClasses[0];
			mappings.JoinedSubclasses.Should().Be.Empty();
			hbmClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Simple", "ComplexType", "Bag", "IdBag", "List", "Set", "Map", "Compo", "OneToOne", "ManyToOne", "Any", "DynamicCompo");
			hbmClass.Properties.Select(p => p.Access).All(x => x.Satisfy(access => access.Contains("field.")));
		}

	    [Test]
		public void WhenMapPropertiesInTheBaseJumpedClassUsingMemberNameThenMapInInherited()
		{
			// ignoring MyClass and using Inherited, as root-class, I will try to map all properties using the base class.
			// NH have to recognize the case and map those properties in the inherited.
			var inspector = new SimpleModelInspector();
			inspector.IsEntity((type, declared) => type == typeof(Inherited));
			inspector.IsRootEntity((type, declared) => type == typeof(Inherited));
			var mapper = new ModelMapper(inspector);
			mapper.Class<MyClass>(mc =>
			                      {
			                      	mc.Id(x => x.Id);
			                      	mc.Property("Simple", map => map.Access(Accessor.Field));
			                      	mc.Property("ComplexType", map => map.Access(Accessor.Field));
			                      	mc.Bag<string>("Bag", y => y.Access(Accessor.Field));
			                      	mc.IdBag<MyCompo>("IdBag", y => y.Access(Accessor.Field));
			                      	mc.List<string>("List", y => y.Access(Accessor.Field));
			                      	mc.Set<string>("Set", y => y.Access(Accessor.Field));
			                      	mc.Map<int, string>("Map", y => y.Access(Accessor.Field));
			                      	mc.OneToOne<Related>("OneToOne", y => y.Access(Accessor.Field));
			                      	mc.ManyToOne<Related>("ManyToOne", y => y.Access(Accessor.Field));
			                      	mc.Any<object>("Any", typeof (int), y => y.Access(Accessor.Field));
			                      	mc.Component("DynamicCompo", new {A = 2}, y => y.Access(Accessor.Field));
			                      	mc.Component<MyCompo>("Compo", y =>
			                      	                               {
			                      	                               	y.Access(Accessor.Field);
			                      	                               	y.Property(c => c.Something);
			                      	                               });
			                      });
			mapper.Class<Inherited>(mc => { });

			HbmMapping mappings = mapper.CompileMappingForAllExplicitlyAddedEntities();
			HbmClass hbmClass = mappings.RootClasses[0];
			mappings.JoinedSubclasses.Should().Be.Empty();
			hbmClass.Properties.Select(p => p.Name).Should().Have.SameValuesAs("Simple", "ComplexType", "Bag", "IdBag", "List", "Set", "Map", "Compo", "OneToOne", "ManyToOne", "Any",
			                                                                   "DynamicCompo");
			hbmClass.Properties.Select(p => p.Access).All(x => x.Satisfy(access => access.Contains("field.")));
		}

		[Test]
		public void WhenMapBagWithWrongElementTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.Bag<int>("Bag", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapIdBagWithWrongElementTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.IdBag<int>("IdBag", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapSetWithWrongElementTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.Set<int>("Set", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapListWithWrongElementTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.Set<int>("Set", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapDictionaryWithWrongKeyTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.Map<string, string>("Map", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapDictionaryWithWrongValueTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.Map<int, int>("Map", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapComponentWithWrongElementTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.Component<object>("Compo", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}


		[Test]
		public void WhenMapOneToOneWithWrongTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.OneToOne<object>("OneToOne", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapManyToOneWithWrongTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.ManyToOne<object>("ManyToOne", y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}

		[Test]
		public void WhenMapAnyWithWrongTypeThenThrows()
		{
			var mapper = new ModelMapper();
			Executing.This(() =>
										 mapper.Class<MyClass>(mc =>
										 {
											 mc.Id(x => x.Id);
											 mc.Any<Related>("Any", typeof(int),y => y.Access(Accessor.Field));
										 })).Should().Throw<MappingException>();
		}
	}
}