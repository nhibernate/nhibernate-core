namespace NHibernate.Test.Stateless
{
	public class Paper
	{
		private int id;
		private string color;

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Color
		{
			get { return color; }
			set { color = value; }
		}
	}
}
