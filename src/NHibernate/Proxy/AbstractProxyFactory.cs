using System;
using System.Collections.Generic;
using System.Linq;
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
		protected virtual bool InterceptsEquals { get; set; }

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
			InterceptsEquals =
				ReflectHelper.OverridesEquals(persistentClass) &&
				(HasUserDeclaredFields(persistentClass) || HasPrivateProperties(persistentClass));
		}

		private static bool HasUserDeclaredFields(System.Type persistentClass)
		{
			return persistentClass.GetFieldsOfHierarchy().Any();
		}

		private static bool HasPrivateProperties(System.Type persistentClass)
		{
			return
				persistentClass
					.GetPropertiesOfHierarchy()
					.Any(p => p.GetMethod?.IsPrivate ?? p.SetMethod?.IsPrivate ?? false);
		}

		public abstract INHibernateProxy GetProxy(object id, ISessionImplementor session);

		public virtual object GetFieldInterceptionProxy(object instanceToWrap)
		{
			throw new NotSupportedException();
		}
	}
}
