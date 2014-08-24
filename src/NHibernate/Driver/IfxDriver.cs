namespace NHibernate.Driver
{
	/// <summary>
	/// A NHibernate Driver for using the Informix DataProvider
	/// </summary>
	public class IfxDriver : ReflectionBasedDriver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IfxDriver"/> class.
		/// </summary>
		/// <exception cref="HibernateException">
		/// Thrown when the <c>IBM.Data.Informix</c> assembly can not be loaded.
		/// </exception>
		public IfxDriver()
			: base(
				"IBM.Data.Informix",
				"IBM.Data.Informix",
				"IBM.Data.Informix.IfxConnection",
				"IBM.Data.Informix.IfxCommand")
		{
		}

		public override bool UseNamedPrefixInSql
		{
			get { return false; }
		}

		public override bool UseNamedPrefixInParameter
		{
			get { return false; }
		}

		public override string NamedPrefix
		{
			get { return ":"; }
		}
	}
}