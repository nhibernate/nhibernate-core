using System;

namespace NHibernate.Test.NHSpecificTest.NH318
{
	/// <summary>
	/// Summary description for NotNullPropertyHolder.
	/// </summary>
	public class NotNullPropertyHolder
	{
		private int _id;
		private String _notNullProperty;

		public NotNullPropertyHolder()
		{
			_id = 0;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public String NotNullProperty
		{
			get { return _notNullProperty; }
			set { _notNullProperty = value; }
		}
	}
}
