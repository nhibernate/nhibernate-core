using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for CompositeId.
	/// </summary>
	public class CompositeId 
	{
		private string _keyString;
		private short _keyShort;
		private System.DateTime _keyDateTime;

		public CompositeId() 
		{
		}

		public CompositeId(string keyString, short keyShort, System.DateTime keyDateTime) {
			_keyString = keyString;
			_keyShort = keyShort;
			_keyDateTime = keyDateTime;
		}

		public string KeyString {
			get { return _keyString;}
			set { _keyString = value;}
		}

//		public short KeyShort {
//			get { return _keyShort;}
//			set {_keyShort = value;}
//		}

		public System.DateTime KeyDateTime {
			get { return _keyDateTime;}
//			set {_keyDateTime = value;}
		}

		public override int GetHashCode() {
			return _keyString.GetHashCode();
		}

		public override bool Equals(object obj) {
			CompositeId otherObj = obj as CompositeId;

			if(otherObj==null) return false;

			if(otherObj.KeyString.Equals(this.KeyString)) return true;

			return false;

		}
	}
}
