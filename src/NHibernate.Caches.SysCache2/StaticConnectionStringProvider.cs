#region Using Statements
using System;
#endregion


namespace NHibernateExtensions.Caches.SysCache {
	/// <summary>
	/// Connection string provider that returns a specified connection string
	/// </summary>
	public class StaticConnectionStringProvider : IConnectionStringProvider  {
	
		#region Private Fields
		/// <summary>specified connection string</summary>
		private string _connectionString;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="StaticConnectionStringProvider"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string that the provider will return</param>
		public StaticConnectionStringProvider(string connectionString){
			this._connectionString = connectionString;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets the name of the default connection string
		/// </summary>
		public string DefaultConnectionName {
			get {
				return string.Empty;
			}
		}
		#endregion

		/// <summary>
		/// Gets the default connection string
		/// </summary>
		/// <returns>Connection string</returns>
		public string GetConnectionString() {
			return this._connectionString;
		}

		/// <summary>
		/// Gets a connnection string by name
		/// </summary>
		/// <param name="name">The name of the connection string to get</param>
		/// <remarks>
		///		<para>The same connection string will be returned whether a name is specified or not.</para>
		/// </remarks>
		public string GetConnectionString(string name) {
			return this._connectionString;
		}
	}
}
