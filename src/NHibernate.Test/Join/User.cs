using System;

namespace NHibernate.Test.Join
{
	public class User : Person
	{
#pragma warning disable 169
		private string _Silly;
#pragma warning restore 169

		private string _Login;
		public virtual string Login
		{
			get { return _Login; }
			set { _Login = value; }
		}

	}
}
