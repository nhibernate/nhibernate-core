using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for CompositeId.
	/// </summary>
	public class CompositeId {
		private string keyString;
		private short keyShort;
		private System.DateTime keyDateTime;

		public CompositeId() {
		}

		public CompositeId(string keyString, short keyShort, System.DateTime keyDateTime) {
			this.keyString = keyString;
			this.keyShort = keyShort;
			this.keyDateTime = keyDateTime;
		}

		public string KeyString {
			get { return this.keyString;}
			set {this.keyString = value;}
		}

		public short KeyShort {
			get { return this.keyShort;}
			set {this.keyShort = value;}
		}

		public System.DateTime KeyDateTime {
			get { return this.keyDateTime;}
			set {this.keyDateTime = value;}
		}

		public override int GetHashCode() {
			return keyString.GetHashCode();
		}

		public override bool Equals(object obj) {
			CompositeId otherObj = obj as CompositeId;

			if(otherObj==null) return false;

			if(otherObj.KeyString.Equals(this.KeyString)) return true;

			return false;

		}
	}
}
