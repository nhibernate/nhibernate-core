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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		public ValidationFailure( string msg ) : base( msg )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="e"></param>
		public ValidationFailure( string msg, Exception e ) : base( msg, e )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		public ValidationFailure( Exception e ) : base( "A validation failure occured", e )
		{
		}

		/// <summary></summary>
		public ValidationFailure() : base( "A validation failure occured" )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ValidationFailure( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}