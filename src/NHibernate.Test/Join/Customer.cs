using System;

namespace NHibernate.Test.Join
{
	public class Customer : Person
	{
		private Employee _Salesperson;
		public virtual Employee Salesperson
		{
			get { return _Salesperson; }
			set { _Salesperson = value; }
		}

		private string _Comments;
		public virtual string Comments
		{
			get { return _Comments; }
			set { _Comments = value; }
		}

	}
}
