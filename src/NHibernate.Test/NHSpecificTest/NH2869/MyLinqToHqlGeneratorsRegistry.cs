using NHibernate.Linq.Functions;
using NHibernate.Util;

namespace NHibernate.Test.NHSpecificTest.NH2869
{
	public class MyLinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
	{
		public MyLinqToHqlGeneratorsRegistry()
		{
			RegisterGenerator(ReflectHelper.GetMethodDefinition(() => MyLinqExtensions.IsOneInDbZeroInLocal(null, null)), new IsTrueInDbFalseInLocalGenerator());
		}
	}
}