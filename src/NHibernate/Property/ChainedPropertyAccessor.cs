using System;

namespace NHibernate.Property
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
			IGetter result = null;
			for (int i = 0; i < chain.Length; i++)
			{
				IPropertyAccessor candidate = chain[i];
				try
				{
					result = candidate.GetGetter(theClass, propertyName);
					return result;
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
			ISetter result = null;
			for (int i = 0; i < chain.Length; i++)
			{
				IPropertyAccessor candidate = chain[i];
				try
				{
					result = candidate.GetSetter(theClass, propertyName);
					return result;
				}
				catch (PropertyNotFoundException)
				{
					//
				}
			}
			throw new PropertyNotFoundException(theClass, propertyName, "setter");
		}
		#endregion
	}
}
