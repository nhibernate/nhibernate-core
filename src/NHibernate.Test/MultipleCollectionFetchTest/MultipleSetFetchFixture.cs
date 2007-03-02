using System;
using System.Collections;

using Iesi.Collections;

using NUnit.Framework;

namespace NHibernate.Test.MultipleCollectionFetchTest
{
	[TestFixture]
	public class MultipleSetFetchFixture : AbstractMultipleCollectionFetchFixture
	{
		protected override IList Mappings
		{
			get { return new string[] {"MultipleCollectionFetchTest.PersonSet.hbm.xml"}; }
		}

		protected override void AddToCollection(ICollection collection, Person person)
		{
			((ISet) collection).Add(person);
		}

		protected override ICollection CreateCollection()
		{
			return new HashedSet();
		}
	}
}
