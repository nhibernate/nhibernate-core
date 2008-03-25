using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for BinaryClass.
	/// </summary>
	public class BinaryClass
	{
		private int _id;
		private byte[] _defaultSize;
		private byte[] _withSize;

		public BinaryClass()
		{
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public byte[] DefaultSize
		{
			get { return _defaultSize; }
			set { _defaultSize = value; }
		}

		public byte[] WithSize
		{
			get { return _withSize; }
			set { _withSize = value; }
		}
	}
}