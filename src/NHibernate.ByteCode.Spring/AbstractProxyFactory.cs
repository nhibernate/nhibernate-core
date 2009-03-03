using System.Reflection;
using Common.Logging;
using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.ByteCode.Spring
{
	/// <summary>
	/// Convenience base class for <see cref="IProxyFactory"/> implementations, 
	/// providing common functionality.
	/// </summary>
	/// <author>Erich Eichinger (Spring.NET Team)</author>
	public abstract class AbstractProxyFactory : IProxyFactory
	{
		protected readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected string EntityName { get; private set; }
		protected System.Type PersistentClass { get; private set; }
		protected System.Type[] Interfaces { get; private set; }
		protected MethodInfo GetIdentifierMethod { get; private set; }
		protected MethodInfo SetIdentifierMethod { get; private set; }
		protected IAbstractComponentType ComponentIdType { get; private set; }

		protected bool IsClassProxy
		{
			get { return Interfaces.Length == 1; }
		}

		public virtual void PostInstantiate(string entityName, System.Type persistentClass, ISet<System.Type> interfaces,
		                                    MethodInfo getIdentifierMethod, MethodInfo setIdentifierMethod,
		                                    IAbstractComponentType componentIdType)
		{
			EntityName = entityName;
			PersistentClass = persistentClass;
			Interfaces = new System.Type[interfaces.Count];

			if (interfaces.Count > 0)
			{
				interfaces.CopyTo(Interfaces, 0);
			}

			GetIdentifierMethod = getIdentifierMethod;
			SetIdentifierMethod = setIdentifierMethod;
			ComponentIdType = componentIdType;
		}

		public abstract INHibernateProxy GetProxy(object id, ISessionImplementor session);
	}
}