using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Linq.Functions
{
	public abstract class BaseHqlGeneratorForType : IHqlGeneratorForType
	{
		protected readonly List<IHqlGeneratorForMethod> MethodRegistry = new List<IHqlGeneratorForMethod>();
		protected readonly List<IHqlGeneratorForProperty> PropertyRegistry = new List<IHqlGeneratorForProperty>();

		#region IHqlGeneratorForType Members

		public void Register(ILinqToHqlGeneratorsRegistry functionRegistry)
		{
			foreach (IHqlGeneratorForMethod generator in MethodRegistry)
			{
				foreach (MethodInfo method in generator.SupportedMethods)
				{
					functionRegistry.RegisterGenerator(method, generator);
				}
			}

			foreach (IHqlGeneratorForProperty generator in PropertyRegistry)
			{
				foreach (MemberInfo property in generator.SupportedProperties)
				{
					functionRegistry.RegisterGenerator(property, generator);
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

		#endregion
	}
}