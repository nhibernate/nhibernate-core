using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate.Util;

namespace NHibernate.Transform
{
	[Serializable]
	public class AliasToBeanConstructorResultTransformer : IResultTransformer
	{
		[NonSerialized]
		private ConstructorInfo _constructor;
		private SerializableConstructorInfo _serializableConstructor;

		public AliasToBeanConstructorResultTransformer(ConstructorInfo constructor)
		{
			_constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_serializableConstructor = SerializableConstructorInfo.Wrap(_constructor);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			_constructor = _serializableConstructor?.Value;
		}

		public object TransformTuple(object[] tuple, string[] aliases)
		{
			try
			{
				return _constructor.Invoke(tuple);
			}
			catch (Exception e)
			{
				throw new QueryException(
					$"could not instantiate: {_constructor.DeclaringType.FullName}",
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
			return Equals(other._constructor, _constructor);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as AliasToBeanConstructorResultTransformer);
		}

		public override int GetHashCode()
		{
			return _constructor.GetHashCode();
		}
	}
}
