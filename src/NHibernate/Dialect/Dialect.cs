using System;

namespace NHibernate.Dialect {

	/// <summary>
	/// Represents a dialect of SQL implemented by a particular RDBMS. Sublcasses
	/// implement Hibernate compatibility with differen systems
	/// </summary>
	/// <remarks>
	/// Subclasses should provide a public default constructor that <c>Register()</c>
	/// a set of type mappings and default Hibernate properties.
	/// </remarks>
	public abstract class Dialect {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Dialect));

		const string DefaultBatchSize = "15";
		const string NoBatch = "0";

		protected Dialect() {
			log.Info( "Using dialect: " + this );
		}

		
		/// <summary>
		/// Characters used for quoting sql identifiers
		/// </summary>
		public const string Quote = "'\"";
	}
}
