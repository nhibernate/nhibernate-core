using System;

namespace NHibernate.Examples.ForumQuestions.OfferExample
{
	/// <summary>
	/// Summary description for OfferType.
	/// </summary>
	public class OfferType
	{
		private int key;
		private string description;

		public OfferType()
		{	
		}

		public int Key 
		{
			get { return key; }
			set { key = value; }
		}

		public string Description 
		{
			get { return description; }
			set { description = value; }
		}
	}
}
