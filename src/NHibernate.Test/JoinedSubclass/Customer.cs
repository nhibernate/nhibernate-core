using System;

namespace NHibernate.Test.JoinedSubclass
{
	/// <summary>
	/// Summary description for Customer.
	/// </summary>
	public class Customer : Person
	{
		private Employee _salesperson;
		private string _comments;

		public Customer()
		{
		}

		public Employee Salesperson
		{
			get { return _salesperson; }
			set { _salesperson = value; }
		}

		public string Comments
		{
			get { return _comments; }
			set { _comments = value; }
		}

	}
}
