using System;

namespace NHibernate.Id
{
	/// <summary>
	/// Thrown by <c>IIdentifierGenerator</c> implementation class when ID generation fails
	/// </summary>
	[Serializable]
	public class IdentifierGenerationException : HibernateException
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public IdentifierGenerationException( string message ) : base( message )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public IdentifierGenerationException( string message, Exception e ) : base( message, e )
		{
		}
	}
}