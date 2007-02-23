using System;

namespace NHibernate.UserTypes.Tests
{
	// this is used by both AnsiString and String since the only real
	// difference is the column in the db.
	public class EmptyStringClass
	{
		private int _id;
		private string _notNullString;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string NotNullString
		{
			get { return _notNullString; }
			set { _notNullString = value; }
		}
	}
}