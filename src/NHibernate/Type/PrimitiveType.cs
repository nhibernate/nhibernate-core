using System;
using NHibernate.Util;

namespace NHibernate.Type {

	/// <summary>
	/// Superclass of primitive / primitive wrapper types.
	/// </summary>
	public abstract class PrimitiveType : ImmutableType, ILiteralType {
		
		public abstract System.Type PrimitiveClass { get; }
	
		public override bool Equals(object x, object y) {
			return ObjectUtils.Equals(x, y);
		}
	
		public override string ToXML(object val) {
			return val.ToString();
		}

		public abstract string ObjectToSQLString(object val);
	}
}