using NHibernate.DependencyInjection.Tests.Model;

namespace NHibernate.DependencyInjection.Tests
{
    public class EntityInjector : Bytecode.IEntityInjector //IEntityInjector
    {
        public object[] GetConstructorParameters(System.Type type)
        {
            if (type == typeof(DependencyInjectionCat)) return new object[] {new CatBehavior()};
            return null;
        }
    }
}