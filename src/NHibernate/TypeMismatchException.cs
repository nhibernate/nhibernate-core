using System;
using System.Runtime.Serialization;

namespace NHibernate
{
	/// <summary> 
	/// Used when a user provided type does not match the expected one 
	/// </summary>
	[Serializable]
	public class TypeMismatchException : HibernateException
	{
		public TypeMismatchException(string message) : base(message) { }
		public TypeMismatchException(string message, Exception inner) : base(message, inner) { }
		// Since v5.6
		[Obsolete("This API supports obsolete formatter-based serialization and will be removed in a future version")]
		protected TypeMismatchException(SerializationInfo info,StreamingContext context): base(info, context) { }
	}
}
