using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Test.NHSpecificTest.NH2931
{
    public class EntityMapping : ClassMapping<Entity>
    {
        public EntityMapping()
        {
            Id(i => i.Id, m => m.Generator(Generators.GuidComb));
        }
    }
    //NOTE: tests may work if the order of mappings is
    // 1) BaseClassMapping
    // 2) DerivedClassMapping
    //but they always fail when
    // 1) DerivedClassMapping
    // 2) BaseClassMapping
    //MappingByCodeTest.CompiledMappings_ShouldNotDependOnAddedOrdering_AddedBy_AddMapping
    // explicitly forces the first ordering to occur, but the most common use is simply
    // typeof(SomeEntity).Assembly.GetTypes() to register everything; as shown in 
    //MappingByCodeTest.CompiledMappings_ShouldNotDependOnAddedOrdering_AddedBy_AddMappings
    public class DerivedClassMapping : JoinedSubclassMapping<DerivedClass>
    {
        public DerivedClassMapping()
        {
            Property(p => p.DerivedProperty);
        }
    }
    public class BaseClassMapping : JoinedSubclassMapping<BaseClass>
    {
        public BaseClassMapping()
        {
            Property(p => p.BaseProperty);
        }
    }
}
