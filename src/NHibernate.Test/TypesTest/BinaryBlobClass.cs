using System;

namespace NHibernate.Test.TypesTest
{
	/// <summary>
	/// Summary description for BinaryBlobClass.
	/// </summary>
	public class BinaryBlobClass
	{
		private int _id;
		private byte[] _blob;
		private string _clob;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public byte[] BinaryBlob
		{
			get { return _blob; }
			set { _blob = value; }
		}
	}
}
