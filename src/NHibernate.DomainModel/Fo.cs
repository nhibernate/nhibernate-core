using System;

namespace NHibernate.DomainModel
{
	public class Fo
	{
		public static Fo NewFo() 
		{
			return new Fo();
		}

		private Fo() {}

		private byte[] _buf;
		private object _serial;
		private long _version;
		private int _x;

		public byte[] Buf
		{
			get { return _buf; }
			set { _buf = value; }
		}

		public object Serial
		{
			get { return _serial; }
			set { _serial = value; }
		}

		
		public long Version
		{
			get { return _version; }
			set { _version = value; }
		}

		public int X
		{
			get { return _x; }
			set { _x = value; }
		}
	}
}
