using System;
using System.Collections;
using System.Collections.Generic;
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

		protected override void AddToCollection(ICollection<Person> persons, Person person)
		{
			((List<Person>) persons).Add(person);
		}

		protected override ICollection<Person> CreateCollection()
		{
			return new List<Person>();
		}
	}
}
