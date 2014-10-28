using System;
using System.Collections;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NUnit.Framework;
using SharpTestsEx;

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

        protected HbmClass CompileClassMapping()
        {
            var mapper = new ModelMapper();
            mapper.AddMapping(typeof(MyClassMap));

            var hbmMapping = mapper.CompileMappingFor(new[] { typeof(MyClass) });
            var hbmClass = hbmMapping.RootClasses[0];

            return hbmClass;
        }

        [Test]
        public void WhenPropertyIsMappedOnRootThenItBelonsToRootTable()
        {
            // <class name="MyClass"">
            var hbmClass = CompileClassMapping();
            hbmClass.Should().Not.Be.Null();

            var rootProperties = hbmClass.Properties;
            // <property name="Code" 
            var hbmPropCode = rootProperties
                .FirstOrDefault(p => p.Name == "Code");

            hbmPropCode.Should().Not.Be.Null();
            hbmPropCode.Should().Be.OfType<HbmProperty>();
        }

        [Test]
        public void WhenDynamicComponentIsMappedOnRootThenItBelonsToRootTable()
        {
            // <class name="MyClass"">
            var hbmClass = CompileClassMapping();
            hbmClass.Should().Not.Be.Null();

            var rootProperties = hbmClass.Properties;
            // <dynamic-component name="Attributes"
            var hbmPropAttributes = rootProperties
                .FirstOrDefault(p => p.Name == "Attributes")
                ;

            hbmPropAttributes.Should().Not.Be.Null();
            hbmPropAttributes.Should().Be.OfType<HbmDynamicComponent>();
        }

        [Test]
        public void WhenRelationIsMappedOnRootThenItBelonsToRootTable()
        {
            // <class name="MyClass"">
            var hbmClass = CompileClassMapping();
            hbmClass.Should().Not.Be.Null();

            var rootProperties = hbmClass.Properties;
            //  <many-to-one name="Relation"
            var hbmPropRelation = rootProperties
                .FirstOrDefault(p => p.Name == "Relation");

            hbmPropRelation.Should().Not.Be.Null();
            hbmPropRelation.Should().Be.OfType<HbmManyToOne>();
        }

        [Test]
        public void WhenJoinedPropertyIsMappedOnJoinThenItBelonsToJoinTable()
        {
            // <class name="MyClass"">
            var hbmClass = CompileClassMapping();
            hbmClass.Should().Not.Be.Null();

            // <join table="JoinedAttributes">
            var hbmJoined = hbmClass.Joins.FirstOrDefault();
            hbmJoined.Should().Not.Be.Null();

            var rootProperties = hbmJoined.Properties;
            // <join table="JoinedAttributes">
            //   <dynamic-component name="Attributes"
            var hbmPropJoinedName = rootProperties
                .FirstOrDefault(p => p.Name == "JoinedName");

            hbmPropJoinedName.Should().Not.Be.Null();
            hbmPropJoinedName.Should().Be.OfType<HbmProperty>();
        }

        [Test]
        public void WhenJoinedRelationIsMappedOnJoinThenItBelonsToJoinTable()
        {
            // <class name="MyClass"">
            var hbmClass = CompileClassMapping();
            hbmClass.Should().Not.Be.Null();

            // <join table="JoinedAttributes">
            var hbmJoined = hbmClass.Joins.FirstOrDefault();
            hbmJoined.Should().Not.Be.Null();

            var rootProperties = hbmJoined.Properties;
            // <join table="JoinedAttributes">
            //   <many-to-one name="JoinedRelation"
            var hbmPropJoinedRelation = rootProperties
                .FirstOrDefault(p => p.Name == "JoinedRelation");

            hbmPropJoinedRelation.Should().Not.Be.Null();
            hbmPropJoinedRelation.Should().Be.OfType<HbmManyToOne>();
        }

        [Test]
        public void WhenJoinedDynamicComponentIsMappedOnJoinThenItBelonsToJoinTable()
        {
            // <class name="MyClass"">
            var hbmClass = CompileClassMapping();
            hbmClass.Should().Not.Be.Null();

            // <join table="JoinedAttributes">
            var hbmJoined = hbmClass.Joins.FirstOrDefault();
            hbmJoined.Should().Not.Be.Null();

            var rootProperties = hbmJoined.Properties;
            // <join table="JoinedAttributes">
            //   <dynamic-component name="JoinedAttributes">
            var hbmPropJoinedAttributes = rootProperties
                .FirstOrDefault(p => p.Name == "JoinedAttributes");

            hbmPropJoinedAttributes.Should().Not.Be.Null();
            hbmPropJoinedAttributes.Should().Be.OfType<HbmDynamicComponent>();
        }

        // [Test]
        public void Expected_XML_Result()
        {
            var mapper = new ModelMapper();
            mapper.AddMapping(typeof(MyClassMap));
            var mapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var mappingXml = mapping.AsString();
            Assert.AreEqual(mappingXml,
@"<?xml version=""1.0"" encoding=""utf-8""?>
<hibernate-mapping xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" namespace=""NHibernate.Test.MappingByCode.ExplicitMappingTests"" assembly=""NHibernate.Test"" xmlns=""urn:nhibernate-mapping-2.2"">
  <class name=""NHibernate.Test.MappingByCode.ExplicitMappingTests.JoinDynamicComponentTests+MyClass"">
    <id type=""Int32"" />
    <property name=""Code"" />
    <many-to-one name=""Relation"" />
    <dynamic-component name=""Attributes"">
      <property name=""IsVisible"" type=""Boolean"" />
      <property name=""Hash"" type=""Guid"" />
      <many-to-one name=""Reference"" />
    </dynamic-component>
    <join table=""JoinedAttributes"">
      <key column=""myclass_key"" />
      <property name=""JoinedName"" />
      <many-to-one name=""JoinedRelation"" />
      <dynamic-component name=""JoinedAttributes"">
        <many-to-one name=""OtherReference"" />
        <property name=""Description"" type=""String"" />
        <property name=""Age"" type=""Int32"" />
      </dynamic-component>
    </join>
  </class>
</hibernate-mapping>");
        }

    }
}