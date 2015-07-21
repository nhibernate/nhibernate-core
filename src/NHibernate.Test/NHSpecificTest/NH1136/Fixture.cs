using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1136
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH1136"; }
		}
		
		protected override void OnTearDown()
		{
			using (ISession s = OpenSession())
			{
				s.Delete("from Address");
				s.Delete("from Person");
				s.Flush();
			}
			
			base.OnTearDown();
		}

		[Test]
		public void Test()
		{
			int id = -1;
			
			using (ISession s = OpenSession())
			{
				var address1 = new Address("60", "EH3 8BE");
				var address2 = new Address("2", "EH6 6JA");
				s.Save(address1);
				s.Save(address2);
				
				var person1 = new Person("'lil old me");
				person1.AddPercentageToFeeMatrix(0, .20m);
				person1.AddPercentageToFeeMatrix(50, .15m);
				person1.AddPercentageToFeeMatrix(100, .1m);
				person1.RegisterChangeOfAddress(new DateTime(2005, 4, 15), address1);
				person1.RegisterChangeOfAddress(new DateTime(2007, 5, 29), address2);
				
				s.Save(person1);
				s.Flush();
				
				id = person1.Id;
			}
			
			using (ISession s = OpenSession())
			{
				var person1 = s.Load<Person>(id);
				person1.RegisterChangeOfAddress(new DateTime(2008, 3, 23), new Address("8", "SS7 1TT"));
				s.Save(person1);
				s.Flush();
			}
		}
	}
}