using System;
using System.Runtime.Serialization;
using NHibernate.Engine;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	[Serializable]
	internal sealed class LiteLazyInitializer : AbstractLazyInitializer
	{
		[NonSerialized]
		private System.Type _persistentClass;
		private SerializableSystemType _serializablePersistentClass;

		internal LiteLazyInitializer(string entityName, object id, ISessionImplementor session, System.Type persistentClass) 
			: base(entityName, id, session)
		{
			_persistentClass = persistentClass;
		}

		public override System.Type PersistentClass => _persistentClass;

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_serializablePersistentClass = SerializableSystemType.Wrap(_persistentClass);
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			_persistentClass = _serializablePersistentClass?.GetSystemType();
		}
	}
}
