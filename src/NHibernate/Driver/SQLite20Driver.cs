using System;

namespace NHibernate.Driver
{
	/// <summary>
	/// NHibernate driver for the System.Data.SQLite data provider for .NET 2.0.
	/// </summary>
	/// <remarks>
	/// <p>
	/// In order to use this driver you must have the System.Data.SQLite.dll assembly available
	/// for NHibernate to load. This assembly includes the SQLite.dll or SQLite3.dll libraries.
	/// </p>    
	/// <p>
	/// You can get the System.Data.SQLite.dll assembly from http://sourceforge.net/projects/sqlite-dotnet2.
	/// </p>
	/// <p>
	/// Please check <a href="http://www.sqlite.org/">http://www.sqlite.org/</a> for more information regarding SQLite.
	/// </p>
	/// </remarks>
	public class SQLite20Driver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of <see cref="SQLiteDriver"/>.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>SQLite.NET</c> assembly can not be loaded.
		/// </exception>
		public SQLite20Driver() : base(
			"System.Data.SQLite",
			"System.Data.SQLite.SQLiteConnection",
			"System.Data.SQLite.SQLiteCommand" )
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
