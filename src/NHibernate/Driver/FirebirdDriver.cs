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
		/// Thrown when the <c>FirebirdSql.Data.FirebirdClient</c> assembly can not be loaded.
		/// </exception>
		public FirebirdDriver() : base(
			"FirebirdSql.Data.FirebirdClient",
			"FirebirdSql.Data.FirebirdClient.FbConnection",
			"FirebirdSql.Data.FirebirdClient.FbCommand")
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