using System;
using System.Runtime.Serialization;
using log4net;

namespace NHibernate
{
	/// <summary>
	/// Indicates failure of an assertion: a possible bug in NHibernate
	/// </summary>
	[ Serializable ]
	public class AssertionFailure : ApplicationException
	{
		/// <summary></summary>
		public AssertionFailure() : base( String.Empty )
		{
			LogManager.GetLogger( typeof( AssertionFailure ) ).Error( "An AssertionFailure occured - this may indicate a bug in NHibernate" );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public AssertionFailure( string message ) : base( message )
		{
			LogManager.GetLogger( typeof( AssertionFailure ) ).Error( "An AssertionFailure occured - this may indicate a bug in NHibernate", this );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public AssertionFailure( string message, Exception e ) : base( message, e )
		{
			LogManager.GetLogger( typeof( AssertionFailure ) ).Error( "An AssertionFailure occured - this may indicate a bug in NHibernate", e );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected AssertionFailure( SerializationInfo info, StreamingContext context ) : base( info, context )
		{
		}
	}
}