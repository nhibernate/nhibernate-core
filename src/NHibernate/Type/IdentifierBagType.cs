using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// Summary description for IdentifierBagType.
	/// </summary>
	public class IdentifierBagType : PersistentCollectionType
	{
		public IdentifierBagType(string role) : base(role)
		{
		}

		public override PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister)
		{
			return new IdentifierBag(session);
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(ICollection); }
		}

		public override PersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new IdentifierBag(session, (ICollection)collection);
		}


		public override PersistentCollection AssembleCachedCollection(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) 
		{
			return new IdentifierBag(session, persister, disassembled, owner);
		}


	}
}
