using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NHibernate.UserTypes;

namespace NHibernate.Type
{
	/// <summary>
	/// A custom type for mapping user-written classes that implement
	/// <see cref="IPersistentCollection" />.
	/// </summary>
	/// <seealso cref="IPersistentCollection"/>
	/// <seealso cref="IUserCollectionType"/>
	[Serializable]
	public class CustomCollectionType : CollectionType
	{
		private readonly IUserCollectionType userType;

		public CustomCollectionType(System.Type userTypeClass, string role, string foreignKeyPropertyName, bool isEmbeddedInXML)
			: base(role, foreignKeyPropertyName, isEmbeddedInXML)
		{
			if (!typeof(IUserCollectionType).IsAssignableFrom(userTypeClass))
			{
				throw new MappingException("Custom type does not implement UserCollectionType: " + userTypeClass.FullName);
			}

			try
			{
				userType = (IUserCollectionType) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(userTypeClass);
			}
			catch (InstantiationException ie)
			{
				throw new MappingException("Cannot instantiate custom type: " + userTypeClass.FullName, ie);
			}
			catch (MemberAccessException mae)
			{
				throw new MappingException("MemberAccessException trying to instantiate custom type: " + userTypeClass.FullName, mae);
			}
		}

		public IUserCollectionType UserType
		{
			get { return userType; }
		}

		public override IPersistentCollection Instantiate(ISessionImplementor session, ICollectionPersister persister, object key)
		{
			return userType.Instantiate(session, persister);
		}

		public override IPersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return userType.Wrap(session, collection);
		}

		public override System.Type ReturnedClass
		{
			get { return userType.Instantiate(-1).GetType(); }
		}

		public override object Instantiate(int anticipatedSize)
		{
			return userType.Instantiate(anticipatedSize);
		}

		public override IEnumerable GetElementsIterator(object collection)
		{
			return userType.GetElements(collection);
		}

		public override bool Contains(object collection, object entity, ISessionImplementor session)
		{
			return userType.Contains(collection, entity);
		}

		public override object IndexOf(object collection, object entity)
		{
			return userType.IndexOf(collection, entity);
		}

		public override object ReplaceElements(object original, object target, object owner, IDictionary copyCache,
		                                       ISessionImplementor session)
		{
			ICollectionPersister cp = session.Factory.GetCollectionPersister(Role);
			return userType.ReplaceElements(original, target, cp, owner, copyCache, session);
		}
	}
}