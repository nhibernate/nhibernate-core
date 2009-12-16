using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
    abstract public class BaseHqlGeneratorForType : IHqlGeneratorForType
    {
        protected readonly List<IHqlGeneratorForMethod> MethodRegistry = new List<IHqlGeneratorForMethod>();
        protected readonly List<IHqlGeneratorForProperty> PropertyRegistry = new List<IHqlGeneratorForProperty>();

        public void Register(FunctionRegistry functionRegistry)
        {
            foreach (var generator in MethodRegistry)
            {
                foreach (var method in generator.SupportedMethods)
                {
                    functionRegistry.RegisterMethodGenerator(method, generator);
                }
            }

            foreach (var generator in PropertyRegistry)
            {
                foreach (var property in generator.SupportedProperties)
                {
                    functionRegistry.RegisterPropertyGenerator(property, generator);
                }
            }
        }

        public virtual bool SupportsMethod(MethodInfo method)
        {
            return false;
        }

        public virtual IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
        {
            throw new NotSupportedException();
        }
    }
}