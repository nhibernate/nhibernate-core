using System;
using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// Generates <see cref="System.Guid"/> values using a strategy suggested Jimmy Nilsson's 
	/// <a href="http://www.informit.com/articles/article.asp?p=25862">article</a>
	/// on <a href="http://www.informit.com">informit.com</a>. 
	/// </summary>
	/// <remarks>
	/// This code was contributed by Donald Mull.
	/// </remarks>
	public class GuidCombGenerator : IIdentifierGenerator
	{
		#region IIdentifierGenerator Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Generate( ISessionImplementor session, object obj )
		{
			return GenerateComb();
		}

		/// <summary></summary>
		private Guid GenerateComb()
		{
			byte[ ] guidArray = Guid.NewGuid().ToByteArray();

			DateTime baseDate = new DateTime( 1900, 1, 1 );
			DateTime now = DateTime.Now;

			// Get the days and milliseconds which will be used to build the byte string 
			TimeSpan days = new TimeSpan( now.Ticks - baseDate.Ticks );
			TimeSpan msecs = new TimeSpan( now.Ticks - ( new DateTime( now.Year, now.Month, now.Day ).Ticks ) );

			// Convert to a byte array 
			// Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
			byte[ ] daysArray = BitConverter.GetBytes( days.Days );
			byte[ ] msecsArray = BitConverter.GetBytes( ( long ) ( msecs.TotalMilliseconds/3.333333 ) );

			// Reverse the bytes to match SQL Servers ordering 
			Array.Reverse( daysArray );
			Array.Reverse( msecsArray );

			// Copy the bytes into the guid 
			Array.Copy( daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2 );
			Array.Copy( msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4 );

			return new Guid( guidArray );
		}

		#endregion
	}
}