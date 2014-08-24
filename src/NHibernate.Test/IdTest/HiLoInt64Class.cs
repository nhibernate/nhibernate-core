using System;

namespace NHibernate.Test.IdTest
{
	/// <summary>
	/// Summary description for StringClobClass.
	/// </summary>
	public class HiLoInt64Class
	{
		private Int64 _id;

		public Int64 Id
		{
			get { return _id; }
			set { _id = value; }
		}
	}
}