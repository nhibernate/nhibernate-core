using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for BasicBinary.
	/// </summary>
	public class BasicBinary
	{
		int _id;
		byte[] _defaultSize;
		byte[] _withSize;

		public BasicBinary()
		{
		}

		public int Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public byte[] DefaultSize
		{
			get {return _defaultSize;}
			set {_defaultSize = value;}
		}

		public byte[] WithSize
		{
			get {return _withSize;}
			set {_withSize = value;}
		}

		
		
	}
}
