using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Linq.Functions
{
	public class DefaultLinqToHqlGeneratorsRegistry : ILinqToHqlGeneratorsRegistry
	{
		private readonly Dictionary<MethodInfo, IHqlGeneratorForMethod> registeredMethods = new Dictionary<MethodInfo, IHqlGeneratorForMethod>();
		private readonly Dictionary<MemberInfo, IHqlGeneratorForProperty> registeredProperties = new Dictionary<MemberInfo, IHqlGeneratorForProperty>();
		private readonly List<IRuntimeMethodHqlGenerator> runtimeMethodHqlGenerators = new List<IRuntimeMethodHqlGenerator>();

		private readonly ConcurrentDictionary<MethodInfo, IHqlGeneratorForMethod> _cachedRuntimeMethodHqlGenerators =
			new ConcurrentDictionary<MethodInfo, IHqlGeneratorForMethod>();

		public DefaultLinqToHqlGeneratorsRegistry()
		{
			RegisterGenerator(new StandardLinqExtensionMethodGenerator());
			RegisterGenerator(new CollectionContainsRuntimeHqlGenerator());
			RegisterGenerator(new DictionaryItemRuntimeHqlGenerator());
			RegisterGenerator(new DictionaryContainsKeyRuntimeHqlGenerator());
			RegisterGenerator(new GenericDictionaryItemRuntimeHqlGenerator());
			RegisterGenerator(new GenericDictionaryContainsKeyRuntimeHqlGenerator());
			RegisterGenerator(new ToStringRuntimeMethodHqlGenerator());
			RegisterGenerator(new LikeGenerator());
			RegisterGenerator(new GetValueOrDefaultGenerator());

			RegisterGenerator(new CompareGenerator());
			this.Merge(new CompareGenerator());

			//NH-3720
			this.Merge(new ConvertToDateTimeGenerator());
			this.Merge(new ConvertToBooleanGenerator());
			this.Merge(new ConvertToInt32Generator());
			this.Merge(new ConvertToDecimalGenerator());
			this.Merge(new ConvertToDoubleGenerator());
			this.Merge(new StartsWithGenerator());
			this.Merge(new EndsWithGenerator());
			this.Merge(new ContainsGenerator());
			this.Merge(new EqualsGenerator());
			this.Merge(new ToUpperGenerator());
			this.Merge(new ToLowerGenerator());
			this.Merge(new SubStringGenerator());
			this.Merge(new IndexOfGenerator());
			this.Merge(new ReplaceGenerator());
			this.Merge(new LengthGenerator());
			this.Merge(new GetCharsGenerator());
			this.Merge(new TrimGenerator());
			this.Merge(new MathGenerator());

			this.Merge(new AnyHqlGenerator());
			this.Merge(new AllHqlGenerator());
			this.Merge(new MinHqlGenerator());
			this.Merge(new MaxHqlGenerator());
			this.Merge(new CollectionContainsGenerator());

			this.Merge(new DateTimePropertiesHqlGenerator());
			this.Merge(new DateTimeNowHqlGenerator());

			this.Merge(new NewGuidHqlGenerator());

			this.Merge(new DecimalAddGenerator());
			this.Merge(new DecimalDivideGenerator());
			this.Merge(new DecimalMultiplyGenerator());
			this.Merge(new DecimalSubtractGenerator());
			this.Merge(new DecimalRemainderGenerator());
			this.Merge(new DecimalNegateGenerator());
			this.Merge(new RoundGenerator());
			this.Merge(new TruncateGenerator());

			var indexerGenerator = new ListIndexerGenerator();
			RegisterGenerator(indexerGenerator);
			this.Merge(indexerGenerator);
		}

		protected bool GetRuntimeMethodGenerator(MethodInfo method, out IHqlGeneratorForMethod methodGenerator)
		{
			methodGenerator = _cachedRuntimeMethodHqlGenerators.GetOrAdd(
				method,
				m =>
					runtimeMethodHqlGenerators
						.Where(g => g.SupportsMethod(m))
						.Select(g => g.GetMethodGenerator(m))
						.FirstOrDefault());

			return methodGenerator != null;
		}

		public virtual bool TryGetGenerator(MethodInfo method, out IHqlGeneratorForMethod generator)
		{
			if (method.IsGenericMethod)
			{
				method = method.GetGenericMethodDefinition();
			}

			if (registeredMethods.TryGetValue(method, out generator)) return true;

			// Not that either.  Let's query each type generator to see if it can handle it
			if (GetRuntimeMethodGenerator(method, out generator)) return true;

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

		public void RegisterGenerator(IRuntimeMethodHqlGenerator generator)
		{
			runtimeMethodHqlGenerators.Add(generator);
			_cachedRuntimeMethodHqlGenerators.Clear();
		}
	}
}
