using System.Reflection;

namespace NHibernate.Linq.Functions
{
	public interface IRuntimeMethodHqlGenerator
	{
		bool SupportsMethod(MethodInfo method);
		IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method);
	}
}