using System;

namespace NHibernate
{
	/// <summary>
	/// Indicates that an expected getter or setter method could not be found on a class
	/// </summary>
	[Serializable]
	public class PropertyNotFoundException : MappingException
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="root"></param>
		public PropertyNotFoundException( string message, Exception root ) : base( message, root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		public PropertyNotFoundException( Exception root ) : base( root )
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public PropertyNotFoundException( string message ) : base( message )
		{
		}
	}
}