using NHibernate.Linq.Functions;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.EntityWithUserTypeCanHaveLinqGenerators
{
	public class EntityWithUserTypePropertyGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public EntityWithUserTypePropertyGeneratorsRegistry()
		{
			RegisterGenerator(ReflectHelper.GetMethod((IExample e) => e.IsEquivalentTo(null)),
							new EntityWithUserTypePropertyIsEquivalentGenerator());
		}
	}
}
