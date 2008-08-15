using System;
using System.Collections;

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

		protected override void AddToCollection(ICollection collection, Person person)
		{
			((ArrayList) collection).Add(person);
		}

		protected override ICollection CreateCollection()
		{
			return new ArrayList();
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
