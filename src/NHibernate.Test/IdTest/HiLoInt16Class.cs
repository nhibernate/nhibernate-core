using System;

namespace NHibernate.Test.IdTest
{
	/// <summary>
	/// Summary description for StringClobClass.
	/// </summary>
	public class HiLoInt16Class
	{
		private Int16 _id;
		private string _clob;

		public Int16 Id
		{
			get { return _id; }
			set { _id = value; }
		}

	}
}

