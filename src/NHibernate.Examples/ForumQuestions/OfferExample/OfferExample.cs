using System;
using System.Collections;

using NHibernate;
using NHibernate.Cfg;

using NHibernate.Examples.ForumQuestions.OfferExample;

using NUnit.Framework;

namespace NHibernate.Examples.ForumQuestions.OfferExample
{
	/// <summary>
	/// Summary description for OfferExample.
	/// </summary>
	[TestFixture]
	public class OfferExample : TestCase
	{

		
		[SetUp]
		public void SetUp() 
		{
			ExportSchema( new string[] { "OfferExample.OfferType.hbm.xml", "OfferExample.Offer.hbm.xml"}, true );
		}

		[TearDown]
		public void TearDown() 
		{
			DropSchema();
		}

		[Test]
		public void TestExample() 
		{
			ISession session = sessions.OpenSession();

			// load some default values
			OfferType[] offerType = new OfferType[3];
			
			offerType[0] = new OfferType();
			offerType[0].Key = 1;
			offerType[0].Description = "somevalue";

			offerType[1] = new OfferType();
			offerType[1].Key = 2;
			offerType[1].Description = "notthatvalue";

			offerType[2] = new OfferType();
			offerType[2].Key = 3;
			offerType[2].Description = "differentvalue";

			Offer[] offer = new Offer[8];
			for(int i = 0; i < offer.Length; i++) 
			{
				offer[i] = new Offer();
				offer[i].Key = i + 1;
				offer[i].MadeBy = "Person " + i;
				offer[i].TypeOfOffer = offerType[0];
				if(i < 5) 
				{
					offer[i].TypeOfOffer = offerType[0];
				}
				else if (i == 5 || i==6)
				{
					offer[i].TypeOfOffer = offerType[1];
				}
				else 
				{
					offer[i].TypeOfOffer = offerType[2];
				}
			}

			for(int i = 0; i < offerType.Length; i++) 
			{
				session.Save(offerType[i]);
			}
			for(int i = 0; i < offer.Length; i++) 
			{
				session.Save(offer[i]);
			}

			session.Flush();
			session.Close();

			session = sessions.OpenSession();
			
			
			string hql = "select offer.MadeBy, offer.TypeOfOffer.Description " +
				"from NHibernate.Examples.ForumQuestions.OfferExample.Offer as offer " +
				"where offer.TypeOfOffer.Description = 'somevalue'";

			IList offers = session.Find(hql);

			Assert.AreEqual(5, offers.Count);

			for(int i=0; i < offers.Count; i++)
			{
				object[] currentRow = (object[])offers[i];
				Assert.AreEqual("somevalue", currentRow[1]);
			}

			session.Close();

		}
	}
}
