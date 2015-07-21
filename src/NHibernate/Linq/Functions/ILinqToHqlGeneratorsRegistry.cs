using System.Reflection;

namespace NHibernate.Linq.Functions
{
	public interface ILinqToHqlGeneratorsRegistry
	{
		bool TryGetGenerator(MethodInfo method, out IHqlGeneratorForMethod generator);
		bool TryGetGenerator(MemberInfo property, out IHqlGeneratorForProperty generator);
		void RegisterGenerator(MethodInfo method, IHqlGeneratorForMethod generator);
		void RegisterGenerator(MemberInfo property, IHqlGeneratorForProperty generator);
		void RegisterGenerator(IRuntimeMethodHqlGenerator generator);
	}
}