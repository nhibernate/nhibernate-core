using System;

#if FEATURE_SERIALIZATION
using System.Runtime.Serialization;
#endif

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
#if FEATURE_SERIALIZATION
		protected TypeMismatchException(SerializationInfo info,StreamingContext context): base(info, context) { }
#endif
	}
}
