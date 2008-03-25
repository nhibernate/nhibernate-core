using System;

namespace NHibernate.Test.Join
{
	public class User : Person
	{
		private string _Silly;

		private string _Login;
		public virtual string Login
		{
			get { return _Login; }
			set { _Login = value; }
		}

	}
}
