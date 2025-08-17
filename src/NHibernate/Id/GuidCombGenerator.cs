using System;
using System.Diagnostics;
using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that generates <see cref="System.Guid"/> values 
	/// using a strategy suggested Jimmy Nilsson's 
	/// <a href="http://www.informit.com/articles/article.asp?p=25862">article</a>
	/// on <a href="http://www.informit.com">informit.com</a>. 
	/// </summary>
	/// <remarks>
	/// <p>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>&lt;generator class="guid.comb" /&gt;</code>
	/// </p>
	/// <p>
	/// The <c>comb</c> algorithm is designed to make the use of GUIDs as Primary Keys, Foreign Keys, 
	/// and Indexes nearly as efficient as ints.
	/// </p>
	/// <p>
	/// This code was contributed by Donald Mull.
	/// </p>
	/// </remarks>
	public partial class GuidCombGenerator : IIdentifierGenerator
	{
		private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate a new <see cref="Guid"/> using the comb algorithm.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier as a <see cref="Guid"/>.</returns>
		public object Generate(ISessionImplementor session, object obj)
		{
			return GenerateComb(Guid.NewGuid(), DateTime.UtcNow);
		}

		/// <summary>
		/// Generate a new <see cref="Guid"/> using the comb algorithm.
		/// </summary>
		protected static Guid GenerateComb(Guid guid, DateTime utcNow)
		{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP2_1_OR_GREATER
			Span<byte> guidArray = stackalloc byte[16];
			guid.TryWriteBytes(guidArray);
#else
			var guidArray = guid.ToByteArray();
#endif
			// Get the days and milliseconds which will be used to build the byte string 
			var ts = new TimeSpan(utcNow.Ticks - BaseDateTicks);
			var days = ts.Days;
			guidArray[10] = (byte) (days >> 8);
			guidArray[11] = (byte) days;
					
			// Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
			var msecs = (long) (utcNow.TimeOfDay.TotalMilliseconds / 3.333333);
			guidArray[12] = (byte) (msecs >> 24);
			guidArray[13] = (byte) (msecs >> 16);
			guidArray[14] = (byte) (msecs >> 8);
			guidArray[15] = (byte) msecs;

			return new Guid(guidArray);
		}

		#endregion
	}
}