using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1948
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		[Test]
		public void CanUseDecimalScaleZero()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person person =
					new Person()
					{
						Age = 50,
						ShoeSize = 10,
						FavouriteNumbers =
							new List<decimal>()
							{
								20,
								30,
								40,
							},
					};

				s.Save(person);
				s.Flush();
				tx.Rollback();
			}
		}
	}
}
