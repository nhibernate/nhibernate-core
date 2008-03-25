using System;

namespace NHibernate.Test.CompositeId
{
	/// <summary>
	/// Summary description for Id.
	/// </summary>
	public class Id
	{
		private string _keyString;
		private short _keyShort;
		private DateTime _keyDateTime;

		public Id()
		{
		}

		public Id(string keyString, short keyShort, DateTime keyDateTime)
		{
			_keyString = keyString;
			_keyShort = keyShort;
			_keyDateTime = keyDateTime;
		}

		public string KeyString
		{
			get { return _keyString; }
			set { _keyString = value; }
		}

//		public short KeyShort {
//			get { return _keyShort;}
//			set {_keyShort = value;}
//		}

		public DateTime KeyDateTime
		{
			get { return _keyDateTime; }
//			set {_keyDateTime = value;}
		}

		public override int GetHashCode()
		{
			return _keyString.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			Id otherObj = obj as Id;

			if (otherObj == null) return false;

			if (otherObj.KeyString.Equals(this.KeyString)) return true;

			return false;
		}
	}
}