using System;

namespace NHibernate.Type {

	/// <summary>
	/// Superclass of nullable immutable types.
	/// </summary>
	public abstract class ImmutableType : NullableType {

		public override object DeepCopyNotNull(object val) {
			return val;
		}

		public override bool IsMutable {
			get {return false; }
		}

		public bool HasNiceEquals {
			get { return true; }
		}
		
	}
}