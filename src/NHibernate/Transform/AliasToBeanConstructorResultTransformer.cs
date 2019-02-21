using System;
using System.Collections;
using System.Reflection;

namespace NHibernate.Transform
{
	[Serializable]
	public class AliasToBeanConstructorResultTransformer : IResultTransformer
	{
		private readonly ConstructorInfo constructor;

		public AliasToBeanConstructorResultTransformer(ConstructorInfo constructor)
		{
			if (constructor == null)
			{
				throw new ArgumentNullException("constructor");
			}
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

		public bool Equals(AliasToBeanConstructorResultTransformer other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.constructor, constructor);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as AliasToBeanConstructorResultTransformer);
		}

		public override int GetHashCode()
		{
			return constructor.GetHashCode();
		}
	}
}