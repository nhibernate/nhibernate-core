using System.Collections.Generic;
using NHibernate.Collection.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Collection;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2278
{
	public class CustomPersistentIdentifierBag<T> : PersistentIdentifierBag<T>, ICustomList<T>
	{
		public CustomPersistentIdentifierBag(ISessionImplementor session)
			: base(session)
		{
		}

		public CustomPersistentIdentifierBag(ISessionImplementor session, ICollection<T> coll)
			: base(session, coll)
		{
		}

		public override bool AfterInitialize(ICollectionPersister persister)
		{
			Assert.That(InternalValues, Is.InstanceOf<CustomList<string>>());
			return base.AfterInitialize(persister);
		}
	}
}
