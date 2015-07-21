namespace NHibernate.Test.Unionsubclass2
{
	public class Customer:Person
	{
		private Employee salesperson;
		private string comments;

		public virtual Employee Salesperson
		{
			get { return salesperson; }
			set { salesperson = value; }
		}

		public virtual string Comments
		{
			get { return comments; }
			set { comments = value; }
		}
	}
}
