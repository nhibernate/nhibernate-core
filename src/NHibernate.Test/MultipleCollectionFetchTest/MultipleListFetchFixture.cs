using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Test.MultipleCollectionFetchTest
{
	[TestFixture]
	public class MultipleListFetchFixture : AbstractMultipleCollectionFetchFixture
	{
		protected override IList Mappings
		{
			get { return new string[] { "MultipleCollectionFetchTest.PersonList.hbm.xml" }; }
		}

		protected override void AddToCollection(ICollection collection, Person person)
		{
			((ArrayList) collection).Add(person);
		}

		protected override ICollection CreateCollection()
		{
			return new ArrayList();
		}
	}
}
