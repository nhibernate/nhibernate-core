using System;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH607
{
	[TestFixture]
	public class Fixture : BugTestCase
	{
		public override string BugNumber
		{
			get { return "NH607"; }
		}

		[Test]
		public void Test()
		{
			PackageParty participant = new PackageParty();
			Package pac = new Package();

			PackageItem packageItem = new PackageItem();
			pac.PackageItems.Add(packageItem);
			packageItem.Package = pac;

			PPP packagePartyParticipant = new PPP();

			packagePartyParticipant.PackageItem = packageItem;
			packagePartyParticipant.PackageParty = participant;

			// make the relation bi-directional
			participant.ParticipatingPackages.Add(packagePartyParticipant);
			packageItem.PackagePartyParticipants.Add(packagePartyParticipant);

			using (ISession session = OpenSession())
			{
				session.Save(pac);
				session.Flush();
			}

			using (ISession session = OpenSession())
			{
				session.Delete("from System.Object o");
				session.Flush();
			}
		}
	}
}