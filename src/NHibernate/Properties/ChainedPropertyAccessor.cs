using System;

namespace NHibernate.Properties
{
	// To be removed in v6.0
	[Serializable]
	[Obsolete("This class has no more usages in NHibernate and will be removed in a future version.")]
	public class ChainedPropertyAccessor : IPropertyAccessor
	{
		private readonly IPropertyAccessor[] chain;

		public ChainedPropertyAccessor(IPropertyAccessor[] chain)
		{
			this.chain = chain;
		}

		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			for (int i = 0; i < chain.Length; i++)
			{
				IPropertyAccessor candidate = chain[i];
				try
				{
					return candidate.GetGetter(theClass, propertyName);
				}
				catch (PropertyNotFoundException)
				{
					// ignore
				}
			}
			throw new PropertyNotFoundException(theClass, propertyName, "getter");
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			for (int i = 0; i < chain.Length; i++)
			{
				IPropertyAccessor candidate = chain[i];
				try
				{
					return candidate.GetSetter(theClass, propertyName);
				}
				catch (PropertyNotFoundException)
				{
					//
				}
			}
			throw new PropertyNotFoundException(theClass, propertyName, "setter");
		}

		public bool CanAccessThroughReflectionOptimizer
		{
			get { return false; }
		}

		#endregion
	}
}
