using System;
using System.Collections;
using System.Net.NetworkInformation;
using NUnit.Framework;

namespace NHibernate.Test.MultipleCollectionFetchTest
{
	[TestFixture]
	[Ignore("Support for non-generic sets removed. To test generic set instead, need to duplicate or port the sister test fixtures too.")]
	public class MultipleSetFetchFixture : AbstractMultipleCollectionFetchFixture
	{
		protected override IList Mappings
		{
			get { return new string[] {"MultipleCollectionFetchTest.PersonSet.hbm.xml"}; }
		}

		protected override void AddToCollection(ICollection collection, Person person)
		{
			//((ISet) collection).Add(person);
			throw new NotImplementedException();
		}

		protected override ICollection CreateCollection()
		{
			throw new NotImplementedException();
			//return new HashedSet();
		}
	}
}
