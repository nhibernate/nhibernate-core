namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the FirebirdSql.Data.Firebird DataProvider.
	/// </summary>
	public class FirebirdDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FirebirdDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>FirebirdSql.Data.Firebird</c> assembly can not be loaded.
		/// </exception>
		public FirebirdDriver() : base(
			"FirebirdSql.Data.Firebird",
			"FirebirdSql.Data.Firebird.FbConnection",
			"FirebirdSql.Data.Firebird.FbCommand")
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
	}
}