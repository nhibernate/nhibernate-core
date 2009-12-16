using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq
{
    public class LinqExtensionMethodAttribute : Attribute
    {
        public string Name { get; private set; }

        public LinqExtensionMethodAttribute()
        {
        }

        public LinqExtensionMethodAttribute(string name)
        {
            Name = name;
        }
    }
}

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
            registry.Register(new ICollectionGenerator());

            return registry;
        }

        private readonly Dictionary<MethodInfo, IHqlGeneratorForMethod> _registeredMethods = new Dictionary<MethodInfo, IHqlGeneratorForMethod>();
        private readonly Dictionary<MemberInfo, IHqlGeneratorForProperty> _registeredProperties = new Dictionary<MemberInfo, IHqlGeneratorForProperty>();
        private readonly List<IHqlGeneratorForType> _typeGenerators = new List<IHqlGeneratorForType>();

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

            // No method generator registered.  Look to see if it's a standard LinqExtensionMethod
            var attr = method.GetCustomAttributes(typeof (LinqExtensionMethodAttribute), false);
            if (attr.Length == 1)
            {
                // It is
                // TODO - cache this?  Is it worth it?
                return new HqlGeneratorForExtensionMethod((LinqExtensionMethodAttribute) attr[0], method);
            }

            // Not that either.  Let's query each type generator to see if it can handle it
            foreach (var typeGenerator in _typeGenerators)
            {
                if (typeGenerator.SupportsMethod(method))
                {
                    return typeGenerator.GetMethodGenerator(method);
                }
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
            _typeGenerators.Add(typeMethodGenerator);
            typeMethodGenerator.Register(this);
        }
    }

    public class HqlGeneratorForExtensionMethod : BaseHqlGeneratorForMethod
    {
        private readonly string _name;

        public HqlGeneratorForExtensionMethod(LinqExtensionMethodAttribute attribute, MethodInfo method)
        {
            _name = string.IsNullOrEmpty(attribute.Name) ? method.Name : attribute.Name;
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var args = visitor.Visit(targetObject)
                              .Union(arguments.Select(a => visitor.Visit(a)))
                              .Cast<HqlExpression>();

            return treeBuilder.MethodCall(_name, args);
        }
    }

    static class UnionExtension
    {
        public static IEnumerable<HqlTreeNode> Union(this HqlTreeNode first, IEnumerable<HqlTreeNode> rest)
        {
            yield return first;

            foreach (var x in rest)
            {
                yield return x;
            }
        }
    }
}