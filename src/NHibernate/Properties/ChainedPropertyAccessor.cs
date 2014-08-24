using System;

namespace NHibernate.Properties
{
	[Serializable]
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
