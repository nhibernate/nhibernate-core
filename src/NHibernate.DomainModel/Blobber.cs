using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Blobber.
	/// </summary>
	public class Blobber
	{
		private int _id;
		private byte[] _blob;
		private string _clob;

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public byte[] Blob
		{
			get { return _blob; }
			set { _blob = value; }
		}

		public string Clob
		{
			get { return _clob; }
			set { _clob = value; }
		}
	}
}