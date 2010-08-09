using System;
using System.Runtime.Serialization;


namespace NHibernate
{
	/// <summary>
	/// Thrown when the application calls <see cref="IQuery.UniqueResult()">IQuery.UniqueResult()</see> 
	/// and the query returned more than one result. Unlike all other NHibernate 
	/// exceptions, this one is recoverable!
	/// </summary>
	[Serializable]
	public class NonUniqueResultException : HibernateException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NonUniqueResultException"/> class.
		/// </summary>
		/// <param name="resultCount">The number of items in the result.</param>
		public NonUniqueResultException(int resultCount)
			: base("query did not return a unique result: " + resultCount.ToString())
		{
			LoggerProvider.LoggerFor(typeof(NonUniqueResultException)).Error("query did not return a unique result: " +
			                                                             resultCount.ToString());
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
		protected NonUniqueResultException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}