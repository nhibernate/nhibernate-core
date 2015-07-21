using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Proxy.Map
{
	/// <summary> Lazy initializer for "dynamic-map" entity representations. </summary>
	[Serializable]
	public class MapLazyInitializer : AbstractLazyInitializer
	{
		public MapLazyInitializer(string entityName, object id, ISessionImplementor session) 
			: base(entityName, id, session) {}

		public IDictionary Map
		{
			get { return (IDictionary) GetImplementation(); }
		}

		public override System.Type PersistentClass
		{
			get
			{
				throw new NotSupportedException("dynamic-map entity representation");
			}
		}
	}
}
