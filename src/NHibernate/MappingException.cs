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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public MappingException( string message, Exception root ) : base( message, root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public MappingException( Exception root ) : base( root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public MappingException( string message ) : base( message )
		{
		}

		/// <summary></summary>
		public MappingException() : base()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected MappingException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}