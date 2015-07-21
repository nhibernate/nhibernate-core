using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.MultipleCollectionFetchTest
{
	[TestFixture]
	public class MultipleBagFetchFixture : AbstractMultipleCollectionFetchFixture
	{
		protected override IList Mappings
		{
			get { return new string[] {"MultipleCollectionFetchTest.PersonBag.hbm.xml"}; }
		}

		protected override void AddToCollection(ICollection<Person> collection, Person person)
		{
			((List<Person>) collection).Add(person);
		}

		protected override ICollection<Person> CreateCollection()
		{
			return new List<Person>();
		}

		protected override void RunLinearJoinFetchTest(Person parent)
		{
			try
			{
				base.RunLinearJoinFetchTest(parent);
				Assert.Fail("Should have failed");
			}
			catch (QueryException e)
			{
				Assert.IsTrue(e.Message.IndexOf("Cannot simultaneously fetch multiple bags") >= 0);
			}
		}

		protected override void RunNonLinearJoinFetchTest(Person person)
		{
			try
			{
				base.RunNonLinearJoinFetchTest(person);
				Assert.Fail("Should have failed");
			}
			catch (QueryException e)
			{
				Assert.IsTrue(e.Message.IndexOf("Cannot simultaneously fetch multiple bags") >= 0);
			}
		}
	}
}
