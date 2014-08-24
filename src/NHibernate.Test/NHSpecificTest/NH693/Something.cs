namespace NHibernate.Test.NHSpecificTest.NH693
{
	public class Something
	{
		private int id;
		private string description;

		public virtual int Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual string Description
		{
			get { return description; }
			set { description = value; }
		}
	}
}
