using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH3024
{
    public class NH3024
    {
        public class Person1
        {
            public int Id { get; set; }
            public string Test { get; set; }
            public Name Name { get; set; }
        }
        public class Name
        {
            public string First { get; set; }
            public string Last { get; set; }
        }

        [Test]
        public void AllowUniqueInComponentMapping()
        {
            var mapper = new ModelMapper();
            mapper.Component<Name>(comp =>
            {
                comp.Unique(true);
                comp.Property(name => name.First);
                comp.Property(name => name.Last);
            });

            mapper.Class<Person1>(cm =>
            {
                cm.Id(person => person.Id, map => map.Generator(Generators.HighLow));
                cm.Property(person => person.Test);
                cm.Component(person => person.Name, comp => { });
            });

            var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var hbmClass = hbmMapping.RootClasses[0];

            var hbmComponents = hbmClass.Properties.OfType<HbmComponent>();
            hbmComponents.Should().Have.Count.EqualTo(1);
            hbmComponents.First(x => x.name.Equals("Name")).unique.Should().Be.True();
        }
    }
}