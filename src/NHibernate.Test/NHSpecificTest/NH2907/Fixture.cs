using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Test.NHSpecificTest.NH2907
{
	/// <summary>
	/// Similar to NH-2113 but with dynamic entity
	/// </summary>
	[TestFixture, Ignore("Not fixed yet.")]
	public class Fixture : BugTestCase
	{
		[Test]
		public void ShouldNotEagerLoadKeyManyToOneWhenOverridingGetHashCode()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var grp = new Group();
				s.Save(grp);

				var loanId = new Dictionary<string, object>
								{
									{"Id", 1},
									{"Group", grp}
								};
				var loan = new Dictionary<string, object>
								{
									{"CompId", loanId}, 
									{"Name", "money!!!"}
								};
				s.Save("Loan", loan);

				tx.Commit();
			}

			bool isInitialized;
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var loan = s.CreateQuery("select l from Loan l")
					 .UniqueResult<IDictionary>();

				var compId = (IDictionary)loan["CompId"];
				var group = compId["Group"];

				group.Should().Not.Be.Null();

				isInitialized = NHibernateUtil.IsInitialized(group);

				tx.Commit();
			}
			isInitialized.Should().Be.False();
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from System.Object");
				tx.Commit();
			}
		}
	}
}
