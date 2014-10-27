using System;
using System.Collections;
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
            public virtual string Name { get; set; }
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

                this.Component(p => p.Attributes,
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
                Join("JoinedAttributes", x =>
                {
                    x.Property(p => p.Name);
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

                ManyToOne(x => x.Relation);
            }
        }

        [Test]
        public void JoinedDynamicComponentShouldBeNestedInJoin()
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
      <property name=""Name"" />
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