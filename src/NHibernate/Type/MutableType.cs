using System;

namespace NHibernate.Type
{
	/// <summary>
	/// Superclass for mutable nullable types.
	/// </summary>
	public abstract class MutableType : NullableType
	{
		public override sealed bool IsMutable {
			get { return true; }
		}
		
		public override bool HasNiceEquals {
			get { return false; } //default ... may be overridden
		}
	}
}