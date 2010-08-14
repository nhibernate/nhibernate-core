using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// NHibernate driver for the SQLite.NET data provider.
	/// <p>
	/// Author: <a href="mailto:ib@stalker.ro"> Ioan Bizau </a>
	/// </p>
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this Driver you must have the SQLite.NET.dll Assembly available for NHibernate to load it.
	/// You must also have the SQLite.dll and SQLite3.dll libraries.
	/// </p>
	/// <p>
	/// Please check <a href="http://www.sqlite.org/"> http://www.sqlite.org/ </a> for more information regarding SQLite.
	/// </p>
	/// </remarks>
	[Obsolete("Use NHibernate.Driver.SQLite20Driver")]
	public class SQLiteDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SQLiteDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>SQLite.NET</c> assembly can not be loaded.
		/// </exception>
		public SQLiteDriver() : base(
			"System.Data.SQLite",
			"SQLite.NET",
			"Finisar.SQLite.SQLiteConnection",
			"Finisar.SQLite.SQLiteCommand")
		{
		}

		public override bool UseNamedPrefixInSql
		{
			get { return true; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return true; }
		}

		public override string NamedPrefix
		{
			get { return "@"; }
		}

		public override bool SupportsMultipleOpenReaders
		{
			get { return false; }
		}
	}
}