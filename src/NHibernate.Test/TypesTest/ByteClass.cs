using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for ByteClass.
	/// </summary>
	public class ByteClass
	{
		int _id;
		byte _byteValue;

		public ByteClass()
		{
		}

		public int Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public byte ByteValue
		{
			get { return _byteValue; }
			set { _byteValue = value; }
		}
		
	}
}