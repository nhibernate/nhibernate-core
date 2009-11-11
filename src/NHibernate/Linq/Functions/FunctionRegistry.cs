using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Linq.Functions
{
    public class FunctionRegistry
    {
        public static FunctionRegistry Initialise()
        {
            var registry = new FunctionRegistry();

            // TODO - could use reflection here
            registry.Register(new QueryableGenerator());
            registry.Register(new StringGenerator());
            registry.Register(new DateTimeGenerator());

            return registry;
        }

        private readonly Dictionary<MethodInfo, IHqlGeneratorForMethod> _registeredMethods = new Dictionary<MethodInfo, IHqlGeneratorForMethod>();
        private readonly Dictionary<MemberInfo, IHqlGeneratorForProperty> _registeredProperties = new Dictionary<MemberInfo, IHqlGeneratorForProperty>();

        public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
        {
            IHqlGeneratorForMethod methodGenerator;

            if (method.IsGenericMethod)
            {
                method = method.GetGenericMethodDefinition();
            }

            if (_registeredMethods.TryGetValue(method, out methodGenerator))
            {
                return methodGenerator;
            }

            throw new NotSupportedException(method.ToString());
        }

        public IHqlGeneratorForProperty GetPropertyGenerator(MemberInfo member)
        {
            IHqlGeneratorForProperty propertyGenerator;

            if (_registeredProperties.TryGetValue(member, out propertyGenerator))
            {
                return propertyGenerator;
            }

            // TODO - different usage pattern to method generator
            return null;
        }

        public void RegisterMethodGenerator(MethodInfo method, IHqlGeneratorForMethod generator)
        {
            _registeredMethods.Add(method, generator);
        }

        public void RegisterPropertyGenerator(MemberInfo property, IHqlGeneratorForProperty generator)
        {
            _registeredProperties.Add(property, generator);
        }

        private void Register(IHqlGeneratorForType typeMethodGenerator)
        {
            typeMethodGenerator.Register(this);
        }
    }
}