using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for StringClobClass.
	/// </summary>
	public class StringClobClass
	{
		private int _id;
		private string _clob;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string StringClob
		{
			get { return _clob; }
			set { _clob = value; }
		}
	}
}