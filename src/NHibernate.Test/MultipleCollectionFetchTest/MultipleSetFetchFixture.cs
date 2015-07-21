using System;
using System.Collections;
using System.Collections.Generic;
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

		protected override void AddToCollection(ICollection<Person> persons, Person person)
		{
			((ISet<Person>) persons).Add(person);
		}

		protected override ICollection<Person> CreateCollection()
		{
			return new HashSet<Person>();
		}
	}
}
