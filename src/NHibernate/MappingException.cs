using System;
using System.Runtime.Serialization;

namespace NHibernate 
{
	/// <summary>
	/// An exception that usually occurs at configuration time, rather than runtime, as a result of
	/// something screwy in the O-R mappings
	/// </summary>
	[Serializable]
	public class MappingException : HibernateException 
	{
		public MappingException(string message, Exception root) : base(message, root) {}

		public MappingException(Exception root) : base(root) { }

		public MappingException(string message) : base(message) {}

		public MappingException() : base() {}

		protected MappingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
