using System;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that returns a string of length
	/// 16.  
	/// </summary>
	/// <remarks>
	/// <para>
	/// This string will NOT consist of only alphanumeric characters.  Use
	/// this only if you don't mind unreadable identifiers.
	/// </para>
	/// <para>
	/// This impelementation was known to be incompatible with Postgres.
	/// </para>
	/// </remarks>
	public class UUIDStringGenerator : IIdentifierGenerator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="cache"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Generate( ISessionImplementor cache, object obj )
		{
			byte[ ] guidInBytes = new byte[16];
			StringBuilder guidBuilder = new StringBuilder( 16, 16 );

			guidInBytes = Guid.NewGuid().ToByteArray();

			// add each item in Byte[] to the string builder
			for( int i = 0; i < guidInBytes.Length; i++ )
			{
				guidBuilder.Append( ( char ) guidInBytes[ i ] );
			}

			return guidBuilder.ToString();
		}
	}
}