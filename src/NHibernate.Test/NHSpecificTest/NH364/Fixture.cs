using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH364
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH364"; }
		}

		[Test]
		public void IdBagIdentity()
		{
			using (ISession s = OpenSession())
			{
				Category cat1 = new Category();
				cat1.Name = "Cat 1";

				Link link1 = new Link();
				link1.Name = "Link 1";
				link1.Categories.Add(cat1);

				s.Save(cat1);
				s.Save(link1);
				s.Flush();

				s.Delete("from Link");
				s.Delete("from Category");
				s.Flush();
			}
		}
	}
}