using System;
using System.Collections;
using System.Reflection;

namespace NHibernate.Transform
{
	[Serializable]
	public class AliasToBeanConstructorResultTransformer : IResultTransformer
	{
		private ConstructorInfo constructor;

		public AliasToBeanConstructorResultTransformer(ConstructorInfo constructor)
		{
			this.constructor = constructor;
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			try
			{
				return constructor.Invoke(tuple);
			}
			catch (Exception e)
			{
				throw new QueryException(
						"could not instantiate: " +
						constructor.DeclaringType.FullName,
						e);
			}
		}

		public IList TransformList(IList collection)
		{
			return collection;
		}
	}
}
