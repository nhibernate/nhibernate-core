using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for BlobberInMemory.
	/// </summary>
	public class BlobberInMemory
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

		public string StringClob
		{
			get { return _clob; }
			set { _clob = value; }
		}


	}
}

