namespace NHibernate.Test.Unconstrained
{
	public class Employee
	{
		private string _id;

		public Employee()
		{
		}

		public Employee(string id) : this()
		{
			_id = id;
		}

		public virtual string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public override string ToString()
		{
			return _id.ToString();
		}
	}
}