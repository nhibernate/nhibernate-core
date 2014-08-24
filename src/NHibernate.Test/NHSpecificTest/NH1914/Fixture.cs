using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1914
{
	[TestFixture]
	public class Fixture : BugTestCase
	{

		[Test]
		public void CascadeInsertAssigned()
		{
			IDS _IDS = new IDS();
			_IDS.Identifier = Guid.NewGuid().ToString();
			_IDS.Name = "IDS";
			_IDS.CRSPLUTs = new Dictionary<String, ListOfHLUT>();
			_IDS.CRSPLUTs.Add("a", new ListOfHLUT());


			HLUT _HLUT = new HLUT();
			_HLUT.Identifier = 1123;
			_HLUT.Name = "HLUT";
			_HLUT.Entries = new List<Entry>();
			_HLUT.Entries.Add(new Entry(1.1, .1));
			_HLUT.Entries.Add(new Entry(2.2, .2));

			_IDS.CRSPLUTs["a"].Values.Add(_HLUT);

			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();
				s.Save(_IDS);
				t.Commit();
			}

			using (ISession s = OpenSession())
			{
				ITransaction t = s.BeginTransaction();
				IDS _IDSRead = s.Load<IDS>(_IDS.Identifier);
				
				Assert.IsNotNull(_IDSRead);
				Assert.IsNotNull(_IDSRead.CRSPLUTs);
				Assert.IsNotNull(_IDSRead.CRSPLUTs["a"]);
				Assert.IsNotNull(_IDSRead.CRSPLUTs["a"].Values[0]);
				Assert.IsNotNull(_IDSRead.CRSPLUTs["a"].Values[0].Entries);

				s.Delete(_IDSRead);
				t.Commit();
			}
		}

	}
}
