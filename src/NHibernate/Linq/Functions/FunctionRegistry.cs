using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
    public class FunctionRegistry
    {
        public static readonly FunctionRegistry Instance = new FunctionRegistry();

        private readonly Dictionary<MethodInfo, IHqlGeneratorForMethod> _registeredMethods = new Dictionary<MethodInfo, IHqlGeneratorForMethod>();
        private readonly Dictionary<MemberInfo, IHqlGeneratorForProperty> _registeredProperties = new Dictionary<MemberInfo, IHqlGeneratorForProperty>();
        private readonly List<IHqlGeneratorForType> _typeGenerators = new List<IHqlGeneratorForType>();

        private FunctionRegistry()
        {
            // TODO - could use reflection here
            Register(new QueryableGenerator());
            Register(new StringGenerator());
            Register(new DateTimeGenerator());
            Register(new ICollectionGenerator());
        }

        public IHqlGeneratorForMethod GetMethodGenerator(MethodInfo method)
        {
            IHqlGeneratorForMethod methodGenerator;

            if (!TryGetMethodGenerator(method, out methodGenerator))
            {
                throw new NotSupportedException(method.ToString());
            }

            return methodGenerator;
        }

        public bool TryGetMethodGenerator(MethodInfo method, out IHqlGeneratorForMethod methodGenerator)
        {
            if (method.IsGenericMethod)
            {
                method = method.GetGenericMethodDefinition();
            }

            if (GetRegisteredMethodGenerator(method, out methodGenerator)) return true;

            // No method generator registered.  Look to see if it's a standard LinqExtensionMethod
            if (GetStandardLinqExtensionMethodGenerator(method, out methodGenerator)) return true;

            // Not that either.  Let's query each type generator to see if it can handle it
            if (GetMethodGeneratorForType(method, out methodGenerator)) return true;

            return false;
        }

        private bool GetMethodGeneratorForType(MethodInfo method, out IHqlGeneratorForMethod methodGenerator)
        {
            methodGenerator = null;

            foreach (var typeGenerator in _typeGenerators.Where(typeGenerator => typeGenerator.SupportsMethod(method)))
            {
                methodGenerator = typeGenerator.GetMethodGenerator(method);
                return true;
            }
            return false;
        }

        private bool GetStandardLinqExtensionMethodGenerator(MethodInfo method, out IHqlGeneratorForMethod methodGenerator)
        {
            methodGenerator = null;

            var attr = method.GetCustomAttributes(typeof(LinqExtensionMethodAttribute), false);

            if (attr.Length == 1)
            {
                // It is
                methodGenerator = new HqlGeneratorForExtensionMethod((LinqExtensionMethodAttribute)attr[0], method);
                return true;
            }
            return false;
        }

        private bool GetRegisteredMethodGenerator(MethodInfo method, out IHqlGeneratorForMethod methodGenerator)
        {
            if (_registeredMethods.TryGetValue(method, out methodGenerator))
            {
                return true;
            }
            return false;
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