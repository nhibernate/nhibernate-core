using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Proxy
{
	/// <summary>
	/// Convenient common implementation for ProxyFactory
	/// </summary>
	public abstract class AbstractProxyFactory: IProxyFactory
	{
		protected virtual string EntityName { get; private set; }
		protected virtual System.Type PersistentClass { get; private set; }
		protected virtual System.Type[] Interfaces { get; private set; }
		protected virtual MethodInfo GetIdentifierMethod { get; private set; }
		protected virtual MethodInfo SetIdentifierMethod { get; private set; }
		protected virtual IAbstractComponentType ComponentIdType { get; private set; }
		protected virtual bool IsClassProxy { get; private set; }
		protected virtual bool OverridesEquals { get; set; }

		public virtual void PostInstantiate(
			string entityName,
			System.Type persistentClass,
			ISet<System.Type> interfaces,
			MethodInfo getIdentifierMethod,
			MethodInfo setIdentifierMethod,
			IAbstractComponentType componentIdType,
			bool isClassProxy)
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
			OverridesEquals = ReflectHelper.OverridesEquals(persistentClass);
			IsClassProxy = isClassProxy;
		}

		[Obsolete("Override PostInstantiate method with isClassProxy parameter instead.")]
		public virtual void PostInstantiate(
			string entityName,
			System.Type persistentClass,
			ISet<System.Type> interfaces,
			MethodInfo getIdentifierMethod,
			MethodInfo setIdentifierMethod,
			IAbstractComponentType componentIdType)
		{
			PostInstantiate(
				entityName,
				persistentClass,
				interfaces,
				getIdentifierMethod,
				setIdentifierMethod,
				componentIdType,
				interfaces.Count == 1);
		}

		public abstract INHibernateProxy GetProxy(object id, ISessionImplementor session);

		// Since 5.3
		[Obsolete("Use ProxyFactoryExtensions.GetFieldInterceptionProxy extension method instead.")]
		public virtual object GetFieldInterceptionProxy(object instanceToWrap)
		{
			throw new NotSupportedException();
		}
	}
}
