using System;
using System.Collections;

using NHibernate.Collection;
using NHibernate.Engine;

namespace NHibernate.Type
{
	/// <summary>
	/// 
	/// </summary>
	public class BagType : PersistentCollectionType
	{
		public BagType(string role) : base(role)
		{
		}

		public override PersistentCollection Instantiate(ISessionImplementor session, CollectionPersister persister) 
		{
			return new Bag(session);
		}

		public override System.Type ReturnedClass 
		{
			get { return typeof(ICollection); }
		}

		public override PersistentCollection Wrap(ISessionImplementor session, object collection)
		{
			return new Bag(session, (ICollection)collection);
		}


		public override PersistentCollection AssembleCachedCollection(ISessionImplementor session, CollectionPersister persister, object disassembled, object owner) 
		{
			return new Bag(session, persister, disassembled, owner);
		}

	}
}