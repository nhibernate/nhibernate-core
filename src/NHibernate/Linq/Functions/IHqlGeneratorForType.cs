using System.Reflection;

namespace NHibernate.Linq.Functions
{
    public interface IHqlGeneratorForType
    {
        void Register(FunctionRegistry functionRegistry);
        bool SupportsMethod(MethodInfo method);
        IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method);
    }
}