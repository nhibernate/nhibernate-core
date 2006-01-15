namespace NHibernate.Test.NHSpecificTest.NH464
{
	public class Promotion
	{
		private int id;
		private string description;
		private PromotionWindow window;

		public Promotion()
		{
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public PromotionWindow Window
		{
			get { return window; }
			set { window = value; }
		}
	}
}