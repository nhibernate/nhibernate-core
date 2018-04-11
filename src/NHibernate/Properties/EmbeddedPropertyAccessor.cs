using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Properties
{
	[Serializable]
	public class EmbeddedPropertyAccessor : IPropertyAccessor
	{
		#region IPropertyAccessor Members

		public IGetter GetGetter(System.Type theClass, string propertyName)
		{
			return new EmbeddedGetter(theClass);
		}

		public ISetter GetSetter(System.Type theClass, string propertyName)
		{
			return new EmbeddedSetter(theClass);
		}

		public bool CanAccessThroughReflectionOptimizer => false;

		#endregion

		[Serializable]
		public sealed class EmbeddedGetter : IGetter
		{
			[NonSerialized]
			private System.Type _clazz;
			private SerializableSystemType _serializableClazz;

			internal EmbeddedGetter(System.Type clazz)
			{
				_clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
			}

			[OnSerializing]
			private void OnSerializing(StreamingContext context)
			{
				_serializableClazz = SerializableSystemType.Wrap(_clazz);
			}

			[OnDeserialized]
			private void OnDeserialized(StreamingContext context)
			{
				_clazz = _serializableClazz?.GetSystemType();
			}

			#region IGetter Members

			public object Get(object target)
			{
				return target;
			}

			public System.Type ReturnType => _clazz;

			public string PropertyName => null;

			public MethodInfo Method => null;

			public object GetForInsert(object owner, IDictionary mergeMap, ISessionImplementor session)
			{
				return Get(owner);
			}

			#endregion

			public override string ToString()
			{
				return string.Format("EmbeddedGetter({0})", _clazz.FullName);
			}
		}

		[Serializable]
		public sealed class EmbeddedSetter : ISetter
		{
			[NonSerialized]
			private System.Type _clazz;
			private SerializableSystemType _serializableClazz;

			internal EmbeddedSetter(System.Type clazz)
			{
				_clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
			}

			[OnSerializing]
			private void OnSerializing(StreamingContext context)
			{
				_serializableClazz = SerializableSystemType.Wrap(_clazz);
			}

			[OnDeserialized]
			private void OnDeserialized(StreamingContext context)
			{
				_clazz = _serializableClazz?.GetSystemType();
			}

			#region ISetter Members

			public void Set(object target, object value)
			{
			}

			public string PropertyName => null;

			public MethodInfo Method => null;

			#endregion

			public override string ToString()
			{
				return string.Format("EmbeddedSetter({0})", _clazz.FullName);
			}
		}

	}
}
