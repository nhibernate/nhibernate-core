using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for GuidClass.
	/// </summary>
	public class GuidClass
	{
		int _id;
		Guid _guidValue;

		public GuidClass()
		{
		}

		public int Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public Guid GuidValue
		{
			get { return _guidValue; }
			set { _guidValue = value; }
		}
		
	}
}