using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NUnit.Framework;

namespace NHibernate.Test.MappingByCode.ExplicitMappingTests
{
	public class JoinDynamicComponentTests
	{
		public class MyClass
		{
			public virtual int Code { get; set; }
			public virtual string JoinedName { get; set; }
			public virtual MyOther Relation { get; set; }
			public virtual MyOther JoinedRelation { get; set; }
			public virtual IDictionary Attributes { get; set; }
			public virtual IDictionary JoinedAttributes { get; set; }
		}

		public class MyOther
		{
			public int Id { get; set; }
		}

		public class MyClassMap : ClassMapping<MyClass>
		{
			public MyClassMap()
			{
				// basic table related properties
				ManyToOne(x => x.Relation);
				Component(p => p.Attributes,
						  new
						  {
							  IsVisible = false,
							  Hash = default(Guid),
							  Reference = default(MyOther)
						  },
						  m =>
						  {
							  m.Property(p => p.IsVisible);
							  m.Property(p => p.Hash);
							  m.ManyToOne(p => p.Reference);
						  });
				Property(x => x.Code);

				// joined table stuff
				Join("JoinedAttributes", x =>
				{
					x.Property(p => p.JoinedName);
					x.Component(p => p.JoinedAttributes,
								new
								{
									OtherReference = default(MyOther),
									Description = string.Empty,
									Age = 0,
								},
								m =>
								{
									m.ManyToOne(p => p.OtherReference);
									m.Property(p => p.Description);
									m.Property(p => p.Age);
								});
					x.ManyToOne(p => p.JoinedRelation);
				});
			}
		}

		private static HbmClass CompileClassMapping()
		{
			var mapper = new ModelMapper();
			mapper.AddMapping(typeof(MyClassMap));

			var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
			var hbmClass = hbmMapping.RootClasses[0];

			return hbmClass;
		}

		[Test]
		public void WhenPropertyIsMappedOnRootThenItBelongsToRootTable()
		{
			// <class name="MyClass"">
			var hbmClass = CompileClassMapping();
			Assert.That(hbmClass, Is.Not.Null);

			var rootProperties = hbmClass.Properties;
			// <property name="Code" 
			var hbmPropCode = rootProperties
				.FirstOrDefault(p => p.Name == "Code");

			Assert.That(hbmPropCode, Is.Not.Null);
			Assert.That(hbmPropCode, Is.TypeOf<HbmProperty>());
		}

		[Test]
		public void WhenDynamicComponentIsMappedOnRootThenItBelongsToRootTable()
		{
			// <class name="MyClass"">
			var hbmClass = CompileClassMapping();
			Assert.That(hbmClass, Is.Not.Null);

			var rootProperties = hbmClass.Properties;
			// <dynamic-component name="Attributes"
			var hbmPropAttributes = rootProperties
				.FirstOrDefault(p => p.Name == "Attributes")
				;

			Assert.That(hbmPropAttributes, Is.Not.Null);
			Assert.That(hbmPropAttributes, Is.TypeOf<HbmDynamicComponent>());
		}

		[Test]
		public void WhenRelationIsMappedOnRootThenItBelongsToRootTable()
		{
			// <class name="MyClass"">
			var hbmClass = CompileClassMapping();
			Assert.That(hbmClass, Is.Not.Null);

			var rootProperties = hbmClass.Properties;
			//  <many-to-one name="Relation"
			var hbmPropRelation = rootProperties
				.FirstOrDefault(p => p.Name == "Relation");

			Assert.That(hbmPropRelation, Is.Not.Null);
			Assert.That(hbmPropRelation, Is.TypeOf<HbmManyToOne>());
		}

		[Test]
		public void WhenJoinedPropertyIsMappedOnJoinThenItBelongsToJoinTable()
		{
			// <class name="MyClass"">
			var hbmClass = CompileClassMapping();
			Assert.That(hbmClass, Is.Not.Null);

			// <join table="JoinedAttributes">
			var hbmJoined = hbmClass.Joins.FirstOrDefault();
			Assert.That(hbmJoined, Is.Not.Null);

			var rootProperties = hbmJoined.Properties;
			// <join table="JoinedAttributes">
			//   <dynamic-component name="Attributes"
			var hbmPropJoinedName = rootProperties
				.FirstOrDefault(p => p.Name == "JoinedName");

			Assert.That(hbmPropJoinedName, Is.Not.Null);
			Assert.That(hbmPropJoinedName, Is.TypeOf<HbmProperty>());
		}

		[Test]
		public void WhenJoinedRelationIsMappedOnJoinThenItBelongsToJoinTable()
		{
			// <class name="MyClass"">
			var hbmClass = CompileClassMapping();
			Assert.That(hbmClass, Is.Not.Null);

			// <join table="JoinedAttributes">
			var hbmJoined = hbmClass.Joins.FirstOrDefault();
			Assert.That(hbmJoined, Is.Not.Null);

			var rootProperties = hbmJoined.Properties;
			// <join table="JoinedAttributes">
			//   <many-to-one name="JoinedRelation"
			var hbmPropJoinedRelation = rootProperties
				.FirstOrDefault(p => p.Name == "JoinedRelation");

			Assert.That(hbmPropJoinedRelation, Is.Not.Null);
			Assert.That(hbmPropJoinedRelation, Is.TypeOf<HbmManyToOne>());
		}

		[Test]
		public void WhenJoinedDynamicComponentIsMappedOnJoinThenItBelongsToJoinTable()
		{
			// <class name="MyClass"">
			var hbmClass = CompileClassMapping();
			Assert.That(hbmClass, Is.Not.Null);

			// <join table="JoinedAttributes">
			var hbmJoined = hbmClass.Joins.FirstOrDefault();
			Assert.That(hbmJoined, Is.Not.Null);

			var rootProperties = hbmJoined.Properties;
			// <join table="JoinedAttributes">
			//   <dynamic-component name="JoinedAttributes">
			var hbmPropJoinedAttributes = rootProperties
				.FirstOrDefault(p => p.Name == "JoinedAttributes");

			Assert.That(hbmPropJoinedAttributes, Is.Not.Null);
			Assert.That(hbmPropJoinedAttributes, Is.TypeOf<HbmDynamicComponent>());
		}
	}
}