using System;

namespace NHibernate.Type {

	/// <summary>
	/// Superclass of primitive / primitive wrapper types.
	/// </summary>
	public abstract class PrimitiveType : ImmutableType, ILiteralType {
		
		public abstract System.Type PrimitiveClass { get; }
	
		public override bool Equals(object x, object y) {
			
			/*
			 Original was:
			 return org.apache.commons.lang.ObjectUtils.equals(x, y);
			
			 This source
			 http://www.generationjava.com/maven/jakarta-commons/lang/apidocs/org/apache/commons/lang/ObjectUtils.html#equals(java.lang.Object,%20java.lang.Object)
			 give this definition
			 
			 "Compares two objects for equality, where either one or both objects may be null"

			 MSDN say that static object.Equals(object objA, object objB) performs
			 "true if objA is the same instance as objB or if both are null references or if objA.Equals(objB) returns true; otherwise, false."
			*/

			return object.Equals(x,y);
		}
	
		public override string ToXML(object val) {
			return val.ToString();
		}

		public abstract string ObjectToSQLString(object val);
	}
}