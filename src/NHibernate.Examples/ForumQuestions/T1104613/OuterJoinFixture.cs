using System;
using System.Collections;

using NUnit.Framework;

namespace NHibernate.Examples.ForumQuestions.T1104613
{
	/// <summary>
	/// Testing outer-join loading with simple classes.  I'm not sure how to
	/// write a Unit Test to see if this is working or not other than just 
	/// look at the sql profilers output.
	/// </summary>
	[TestFixture]
	public class OuterJoinFixture : TestCase
	{
		[SetUp]
		public void SetUp()
		{
			ExportSchema(new string[] {"T1104613.A.hbm.xml"}, true);
		}

		[Test]
		public void LoadWithOuterJoin()
		{
			ISession s = sessions.OpenSession();

			const int numOfOjs = 5;

			A theA = new A();
			AManyToOne mto = new AManyToOne();
			IList ojList = new ArrayList(numOfOjs);

			theA.Name = "the A";
			mto.Name = "many-to-one";

			theA.ManyToOne = mto;

			for (int i = 0; i < numOfOjs; i++)
			{
				AOuterJoin aoj = new AOuterJoin();
				aoj.Name = "the oj list " + i;
				ojList.Insert(i, aoj);
			}

			theA.OuterJoins = ojList;

			for (int i = 0; i < numOfOjs; i++)
			{
				s.Save(ojList[i]);
			}
			s.Save(mto);
			s.Save(theA);

			s.Flush();
			s.Close();

			s = sessions.OpenSession();
			theA = (A) s.Load(typeof(A), theA.Key);

			s.Close();
		}
	}
}