using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Visitors;

namespace NHibernate.Linq.Functions
{
	public class DefaultLinqToHqlGeneratorsRegistry : ILinqToHqlGeneratorsRegistry
	{
		private readonly Dictionary<MethodInfo, IHqlGeneratorForMethod> registeredMethods = new Dictionary<MethodInfo, IHqlGeneratorForMethod>();
		private readonly Dictionary<MemberInfo, IHqlGeneratorForProperty> registeredProperties = new Dictionary<MemberInfo, IHqlGeneratorForProperty>();
		private readonly List<IHqlGeneratorForType> typeGenerators = new List<IHqlGeneratorForType>();

		public DefaultLinqToHqlGeneratorsRegistry()
		{
			// TODO - could use reflection here
			Register(new StandardLinqExtensionMethodGenerator());
			Register(new QueryableGenerator());
			Register(new StringGenerator());
			Register(new DateTimeGenerator());
			Register(new ICollectionGenerator());
		}

		protected bool GetMethodGeneratorForType(MethodInfo method, out IHqlGeneratorForMethod methodGenerator)
		{
			methodGenerator = null;

			foreach (var typeGenerator in typeGenerators.Where(typeGenerator => typeGenerator.SupportsMethod(method)))
			{
				methodGenerator = typeGenerator.GetMethodGenerator(method);
				return true;
			}
			return false;
		}

		protected bool GetRegisteredMethodGenerator(MethodInfo method, out IHqlGeneratorForMethod methodGenerator)
		{
			if (registeredMethods.TryGetValue(method, out methodGenerator))
			{
				return true;
			}
			return false;
		}

		public virtual bool TryGetGenerator(MethodInfo method, out IHqlGeneratorForMethod generator)
		{
			if (method.IsGenericMethod)
			{
				method = method.GetGenericMethodDefinition();
			}

			if (GetRegisteredMethodGenerator(method, out generator)) return true;

			// Not that either.  Let's query each type generator to see if it can handle it
			if (GetMethodGeneratorForType(method, out generator)) return true;

			return false;
		}

		public virtual bool TryGetGenerator(MemberInfo property, out IHqlGeneratorForProperty generator)
		{
			return registeredProperties.TryGetValue(property, out generator);
		}

		public virtual void RegisterGenerator(MethodInfo method, IHqlGeneratorForMethod generator)
		{
			registeredMethods.Add(method, generator);
		}

		public virtual void RegisterGenerator(MemberInfo property, IHqlGeneratorForProperty generator)
		{
			registeredProperties.Add(property, generator);
		}

		protected void Register(IHqlGeneratorForType typeMethodGenerator)
		{
			typeGenerators.Add(typeMethodGenerator);
			typeMethodGenerator.Register(this);
		}
	}
}