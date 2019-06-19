﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH2693
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		private List<FirstLevel> _firstLevels;

		protected override void OnSetUp()
		{
			base.OnSetUp();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var firstLevel1 = new FirstLevel();

					var secondLevel1 = new SecondLevelComponent { FirstLevel = firstLevel1 };
					firstLevel1.SecondLevels.Add(secondLevel1);

					var thirdLevel1 = new SpecificThirdLevel();
					secondLevel1.ThirdLevel = thirdLevel1;
					secondLevel1.SpecificThirdLevel = thirdLevel1;

					var fourthLevel1 = new FourthLevel { SomeString = "first", SpecificThirdLevel = thirdLevel1 };
					thirdLevel1.FourthLevels.Add(fourthLevel1);
					var fourthLevel2 = new FourthLevel { SomeString = "second", SpecificThirdLevel = thirdLevel1 };
					thirdLevel1.FourthLevels.Add(fourthLevel2);


					var firstLevel2 = new FirstLevel();

					var secondLevel2 = new SecondLevelComponent { FirstLevel = firstLevel2 };
					firstLevel2.SecondLevels.Add(secondLevel2);

					var thirdLevel2 = new SpecificThirdLevel();
					secondLevel2.ThirdLevel = thirdLevel2;
					secondLevel2.SpecificThirdLevel = thirdLevel2;

					var fourthLevel3 = new FourthLevel { SomeString = "third", SpecificThirdLevel = thirdLevel2 };
					thirdLevel2.FourthLevels.Add(fourthLevel3);
					var fourthLevel4 = new FourthLevel { SomeString = "fourth", SpecificThirdLevel = thirdLevel2 };
					thirdLevel2.FourthLevels.Add(fourthLevel4);


					var firstLevel3 = new FirstLevel();

					var secondLevel3 = new SecondLevelComponent { FirstLevel = firstLevel3 };
					firstLevel3.SecondLevels.Add(secondLevel3);

					var thirdLevel3 = new SpecificThirdLevel();
					secondLevel3.ThirdLevel = thirdLevel3;
					secondLevel3.SpecificThirdLevel = thirdLevel3;


					session.Save(thirdLevel1);
					session.Save(thirdLevel2);
					session.Save(thirdLevel3);
					session.Save(firstLevel1);
					session.Save(firstLevel2);
					session.Save(firstLevel3);

					_firstLevels = new List<FirstLevel>();
					_firstLevels.Add(firstLevel1);
					_firstLevels.Add(firstLevel2);
					_firstLevels.Add(firstLevel3);

					tx.Commit();
				}
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from FourthLevel");
					session.Delete("from ThirdLevel");
					session.Delete("from FirstLevel");
					tx.Commit();
				}
			}
		}

		[Test]
		public async Task _5_Querying_BasedOnFourthLevelExistence_ByUsingSpecificThirdLevelProperty_ShouldReturnSameEntitiesAsLinqToObjectsAsync()
		{
			var expected = _firstLevels
			   .Where(first => first.SecondLevels
				  .Any(second => second.SpecificThirdLevel.FourthLevels
					 .Any()
				  )
			   )
			   .ToList();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var result = await (session.Query<FirstLevel>()
					   .Where(first => first.SecondLevels
						  .Any(second => second.SpecificThirdLevel.FourthLevels
							 .Any()
						  )
					   )
					   .ToListAsync());

					Assert.AreEqual(expected.Count, result.Count);
					Assert.IsTrue(result
					   .All(f => f.SecondLevels
						  .Any(second => second.SpecificThirdLevel.FourthLevels
							 .Any()
						  )
					   )
					);
				}
			}
		}

		[Test]
		public async Task _6_Querying_BasedOnFourthLevelProperty_ByUsingSpecificThirdLevelProperty_ShouldReturnSameEntitiesAsLinqToObjectsAsync()
		{
			var expected = _firstLevels
			   .Where(first => first.SecondLevels
				  .Any(second => second.SpecificThirdLevel.FourthLevels
					 .Any(fourth => fourth.SomeString == "first")
				  )
			   )
			   .ToList();

			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var result = await (session.Query<FirstLevel>()
					   .Where(first => first.SecondLevels
						  .Any(second => second.SpecificThirdLevel.FourthLevels
							 .Any(fourth => fourth.SomeString == "first")
						  )
					   )
					   .ToListAsync());

					Assert.AreEqual(expected.Count, result.Count);
					Assert.IsTrue(result
					   .All(f => f.SecondLevels
						  .Any(second => second.SpecificThirdLevel.FourthLevels
							 .Any(fourth => fourth.SomeString == "first")
						  )
					   )
					);
				}
			}
		}
	}
}
