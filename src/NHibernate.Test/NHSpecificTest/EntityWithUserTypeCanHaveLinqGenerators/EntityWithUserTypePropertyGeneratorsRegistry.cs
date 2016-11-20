using NHibernate.Linq;
using NHibernate.Linq.Functions;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public class EntityWithUserTypePropertyGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public EntityWithUserTypePropertyGeneratorsRegistry()
		{
			RegisterGenerator(ReflectionHelper.GetMethod((IExample e) => e.IsEquivalentTo(null)),
							new EntityWithUserTypePropertyIsEquivalentGenerator());
		}
	}
}
