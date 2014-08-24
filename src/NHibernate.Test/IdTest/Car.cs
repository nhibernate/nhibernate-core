namespace NHibernate.Test.IdTest
{
	public class Car
	{
		private long id;
		private string color;

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Color
		{
			get { return color; }
			set { color = value; }
		}
	}
}