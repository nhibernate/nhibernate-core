using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for BooleanClass.
	/// </summary>
	public class BooleanClass
	{
		private int _id;
		private bool _booleanValue;

		public BooleanClass()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public bool BooleanValue
		{
			get { return _booleanValue; }
			set { _booleanValue = value; }
		}
	}
}