using System;
using System.Runtime.Serialization;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// Thrown when the application calls <tt>Query.uniqueResult()</tt> and
	/// the query returned more than one result. Unlike all other Hibernate 
	/// exceptions, this one is recoverable!
	/// </summary>
	[Serializable]
	public class NonUniqueResultException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueResultException"/> class.
		/// </summary>
		public NonUniqueResultException() : this( 0 )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueResultException"/> class.
		/// </summary>
		/// <param name="resultCount">The number of items in the result.</param>
		public NonUniqueResultException( int resultCount ) : base( "query did not return a unique result: " + resultCount.ToString() )
		{
			LogManager.GetLogger( typeof( NonUniqueResultException ) ).Error( "query did not return a unique result: " + resultCount.ToString(), this );
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LazyInitializationException"/> class.
		/// </summary>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public NonUniqueResultException( Exception innerException ) : base( "NHibernate non-unique result problem", innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueResultException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error. </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception. If the innerException parameter 
		/// is not a null reference, the current exception is raised in a catch block that handles 
		/// the inner exception.
		/// </param>
		public NonUniqueResultException( string message, Exception innerException ) : base( message, innerException )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueResultException"/> class
		/// with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
		/// </param>
		protected NonUniqueResultException( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}