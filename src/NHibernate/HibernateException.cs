using System;
using System.Runtime.Serialization;

namespace NHibernate 
{
	
	/// <summary>
	/// Any exception that occurs in the O-R persistence layer.
	/// </summary>
	/// <remarks>Exceptions that occur in the database layer are left as native exceptions</remarks>
	[Serializable]
	public class HibernateException : ApplicationException 
	{
		public HibernateException() : base (String.Empty) { }

		public HibernateException(Exception e) : base(String.Empty, e) { }

		public HibernateException(string str, Exception e) : base(str, e) { }

		public HibernateException(string str) : base(str) { }

		protected HibernateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
