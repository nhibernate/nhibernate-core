using System;

namespace NHibernate.Examples.ForumQuestions.OfferExample
{
	/// <summary>
	/// Summary description for Offer.
	/// </summary>
	public class Offer
	{
		private int key;
		private OfferType typeOfOffer;
		private string madeBy;

		public Offer()
		{
		}

		public int Key 
		{
			get { return key; }
			set { key = value; }
		}

		public string MadeBy 
		{
			get { return madeBy; }
			set { madeBy = value; }
		}

		public OfferType TypeOfOffer 
		{
			get { return typeOfOffer; }
			set { typeOfOffer = value; }
		}

	}
}
