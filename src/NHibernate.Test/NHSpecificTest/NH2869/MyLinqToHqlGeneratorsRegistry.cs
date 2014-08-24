using NHibernate.Linq;
using NHibernate.Linq.Functions;

namespace NHibernate.Test.NHSpecificTest.NH2869
{
	public class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public MyLinqToHqlGeneratorsRegistry()
		{
			RegisterGenerator(ReflectionHelper.GetMethodDefinition(() => MyLinqExtensions.IsOneInDbZeroInLocal(null, null)), new IsTrueInDbFalseInLocalGenerator());
		}
	}
}