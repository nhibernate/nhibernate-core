namespace NHibernate.Test
{
	/// <summary>
	/// A disposible object that taps into the "NHibernate.SQL" logger and
	/// collection the log entries being logged.  This class should be used
	/// with a C# using-statement
	/// </summary>
	public class SqlLogSpy : LogSpy
	{
		public SqlLogSpy()
			: base("NHibernate.SQL")
		{}
	}
}
