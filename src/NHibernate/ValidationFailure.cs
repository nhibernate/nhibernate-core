using System;
using System.Runtime.Serialization;

namespace NHibernate 
{
	/// <summary>
	/// Thrown from <c>IValidatable.Validate()</c> when an invariant was violated. Some applications
	/// might subclass this exception in order to provide more information about the violation
	/// </summary>
	[Serializable]
	public class ValidationFailure : HibernateException 
	{
		public ValidationFailure(string msg) : base(msg) {}

		public ValidationFailure(string msg, Exception e) : base(msg, e) {}

		public ValidationFailure(Exception e) : base("A validation failure occured", e) {}

		public ValidationFailure() : base("A validation failure occured") {}

		protected ValidationFailure(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
