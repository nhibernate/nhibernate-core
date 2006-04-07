using System;

namespace NHibernate.Test.IdTest
{
	/// <summary>
	/// Summary description for StringClobClass.
	/// </summary>
	public class HiLoInt64Class
	{
		private Int64 _id;
		private string _clob;

		public Int64 Id
		{
			get { return _id; }
			set { _id = value; }
		}

	}
}

